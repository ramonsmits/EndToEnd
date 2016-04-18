#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Logging;
using Tests.Permutations;
using System.Collections.Generic;
using System.IO;
using Common.Scenarios;
using NServiceBus.Performance;

public abstract class BaseRunner : IConfigurationSource, IContext
{
    readonly ILog Log = LogManager.GetLogger("BaseRunner");

#if Version5
    protected IBus EndpointInstance { get; private set; }
#else
    protected IEndpointInstance EndpointInstance { get; private set; }
#endif

    public Permutation Permutation { get; private set; }
    public string EndpointName { get; private set; }

    public virtual void Execute(Permutation permutation, string endpointName)
    {
        Permutation = permutation;
        EndpointName = endpointName;

        ThrowIfPermutationNotAllowed();

        CreateSeedData();

        EndpointInstance = CreateEndpoint();

        try
        {
            Start();
            Log.InfoFormat("Warmup: {0}", Settings.WarmupDuration);
            System.Threading.Thread.Sleep(Settings.WarmupDuration);
            Statistics.Instance.Reset();
            Log.InfoFormat("Run: {0}", Settings.RunDuration);
            System.Threading.Thread.Sleep(Settings.RunDuration);
            Statistics.Instance.Dump();
            Stop();
        }
        finally
        {
#if Version5
            using (EndpointInstance) { }
#else
            EndpointInstance.Stop().GetAwaiter().GetResult();
#endif
        }
    }

    void ThrowIfPermutationNotAllowed()
    {
        var thrower = this as IThrowIfPermutationIsNotAllowed;
        if (thrower == null) return;

        thrower.ThrowIfPermutationIsNotAllowed(Permutation);
    }

    protected virtual void Start()
    {
    }

    protected virtual void Stop()
    {
    }

    private void CreateSeedData()
    {
        var seedCreator = this as ICreateSeedData;
        if (seedCreator == null) return;

        if (seedCreator.SeedSize == 0) throw new InvalidOperationException("SeedSize was not set.");

        CreateQueues();

        var sendonlyInstance = CreateSendOnlyEndpoint();

        try
        {
            Parallel.For(0, seedCreator.SeedSize, (i, state) =>
            {
                if (i % 5000 == 0)
                    Log.InfoFormat("Seeded {0} messages.", i);

                ((ICreateSeedData)this).SendMessage(sendonlyInstance);
            });
            Log.InfoFormat("Seeded total of {0} messages.", seedCreator.SeedSize);
        }
        finally
        {
#if Version5
            using (sendonlyInstance) { }
#else
            sendonlyInstance.Stop().GetAwaiter().GetResult();
#endif
        }
    }

#if Version5
    void CreateQueues()
    {
        var configuration = CreateConfiguration();
        configuration.PurgeOnStartup(true);
        var createQueuesBus = Bus.Create(configuration).Start();
        createQueuesBus.Dispose();
    }

    ISendOnlyBus CreateSendOnlyEndpoint()
    {
        var configuration = CreateConfiguration();
        return Bus.CreateSendOnly(configuration);
    }

    IBus CreateEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.CustomConfigurationSource(this);

        if (QueuesWerePurgedWhenSeedingData())
            configuration.PurgeOnStartup(false);
        else
            configuration.PurgeOnStartup(true);

        return Bus.Create(configuration).Start();
    }

    private bool QueuesWerePurgedWhenSeedingData()
    {
        if (this is ICreateSeedData) return true;
        return false;
    }

    BusConfiguration CreateConfiguration()
    {
        var configuration = new Configuration();
        configuration.EndpointName(EndpointName);
        configuration.EnableInstallers();

        var scanableTypes = GetTypesToInclude();
        configuration.TypesToScan(scanableTypes);

        configuration.ApplyProfiles(this);

        return configuration;
    }
#else
    void CreateQueues()
    {
        var configuration = CreateConfiguration();
        configuration.PurgeOnStartup(true);
        Endpoint.Create(configuration).GetAwaiter().GetResult();
    }

    IEndpointInstance CreateSendOnlyEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.SendOnly();
        return Endpoint.Start(configuration).GetAwaiter().GetResult();
    }

    IEndpointInstance CreateEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.PurgeOnStartup(false);
        configuration.CustomConfigurationSource(this);

        return Endpoint.Start(configuration).GetAwaiter().GetResult();
    }

    Configuration CreateConfiguration()
    {
        var configuration = new Configuration(EndpointName);
        configuration.EnableInstallers();

        configuration.ExcludeTypes(GetTypesToExclude().ToArray());

        configuration.ApplyProfiles(this);

        return configuration;
    }
#endif

    List<Type> GetTypesToInclude()
    {
        var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var asm = new NServiceBus.Hosting.Helpers.AssemblyScanner(location).GetScannableAssemblies();

        var allTypes = (from a in asm.Assemblies
                        from b in a.GetLoadableTypes()
                        select b).ToList();

        IEnumerable<Type> allTypesToExclude = GetTypesToExclude(allTypes);
        var finalInternalListToScan = allTypes.Except(allTypesToExclude);

        return finalInternalListToScan.ToList();
    }

    IEnumerable<Type> GetTypesToExclude()
    {
        return GetTypesToExclude(Assembly.GetAssembly(this.GetType()).GetTypes());
    } 

    private IEnumerable<Type> GetTypesToExclude(IEnumerable<Type> allTypes)
    {
        var allTypesToExclude = (from t in allTypes
            where (t.IsSubclassOf(typeof(BaseRunner)) || t.IsSubclassOf(typeof(LoopRunner)) || t == typeof(LoopRunner.Handler)) && t != this.GetType()
            select t).ToList();

        Log.InfoFormat("This is test {0}, excluding :", this.GetType().Name);
        foreach (var theType in allTypesToExclude)
        {
            Log.InfoFormat("- {0}", theType.Name);
        }
        return allTypesToExclude;
    }

    public T GetConfiguration<T>() where T : class, new()
    {
        IConfigureUnicastBus configureUnicastBus;

        if (typeof(T) == typeof(UnicastBusConfig) && null != (configureUnicastBus = this as IConfigureUnicastBus))
        {
            //read from existing config 
            var config = (UnicastBusConfig)ConfigurationManager.GetSection(typeof(UnicastBusConfig).Name);
            if (config != null) throw new InvalidOperationException("UnicastBUs Configuration should be in code using IConfigureUnicastBus interface.");

            return new UnicastBusConfig
            {
                MessageEndpointMappings = configureUnicastBus.GenerateMappings()
            } as T;
        }

        return ConfigurationManager.GetSection(typeof(T).Name) as T;
    }
}