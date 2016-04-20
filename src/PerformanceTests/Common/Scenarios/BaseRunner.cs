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
using Common.Scenarios;

public abstract class BaseRunner : IConfigurationSource, IContext
{
    readonly ILog Log = LogManager.GetLogger("BaseRunner");

    public Permutation Permutation { get; private set; }
    public string EndpointName { get; private set; }
    protected ISession Session { get; private set; }

    protected byte[] Data { private set; get; }

    public virtual void Execute(Permutation permutation, string endpointName)
    {

        Permutation = permutation;
        EndpointName = endpointName;

        InitData();

        ThrowIfPermutationNotAllowed();

        CreateSeedData();

        CreateEndpoint();

        try
        {
            Start();
            Log.InfoFormat("Warmup: {0}", Settings.WarmupDuration);
            System.Threading.Thread.Sleep(Settings.WarmupDuration);
            Statistics.Instance.Reset(GetType().Name);
            Log.InfoFormat("Run: {0}", Settings.RunDuration);
            System.Threading.Thread.Sleep(Settings.RunDuration);
            Statistics.Instance.Dump();
            Stop();
        }
        finally
        {
            Session.Close();
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

    void CreateSeedData()
    {
        var seedCreator = this as ICreateSeedData;
        if (seedCreator == null) return;

        if (seedCreator.SeedSize == 0) throw new InvalidOperationException("SeedSize was not set.");

        CreateQueues();

        CreateSendOnlyEndpoint();

        try
        {
            Parallel.For(0, seedCreator.SeedSize, (i, state) =>
            {
                if (i % 5000 == 0)
                    Log.InfoFormat("Seeded {0} messages.", i);

                ((ICreateSeedData)this).SendMessage(Session);
            });
            Log.InfoFormat("Seeded total of {0} messages.", seedCreator.SeedSize);
        }
        finally
        {
            Session.Close();
        }
    }

#if Version5
    void CreateQueues()
    {
        var configuration = CreateConfiguration();
        configuration.PurgeOnStartup(true);
        using (Bus.Create(configuration).Start()){}
    }

    void CreateSendOnlyEndpoint()
    {
        var configuration = CreateConfiguration();
        var instance = Bus.CreateSendOnly(configuration);
        Session = new Session(instance);
    }

    void CreateEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.CustomConfigurationSource(this);

        if (QueuesWerePurgedWhenSeedingData())
            configuration.PurgeOnStartup(false);
        else
            configuration.PurgeOnStartup(true);

        Session = new Session(Bus.Create(configuration).Start());
    }

    bool QueuesWerePurgedWhenSeedingData()
    {
        return this is ICreateSeedData;
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

    List<Type> GetTypesToInclude()
    {
        var location = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var asm = new NServiceBus.Hosting.Helpers.AssemblyScanner(location).GetScannableAssemblies();

        var allTypes = (from a in asm.Assemblies
                        from b in a.GetLoadableTypes()
                        select b).ToList();

        var allTypesToExclude = GetTypesToExclude(allTypes);
        
        var finalInternalListToScan = allTypes.Except(allTypesToExclude);

        return finalInternalListToScan.ToList();
    }
#else
    void CreateQueues()
    {
        var configuration = CreateConfiguration();
        configuration.PurgeOnStartup(true);
        var instance = Endpoint.Start(configuration).GetAwaiter().GetResult();
        instance.Stop().GetAwaiter().GetResult();
    }

    void CreateSendOnlyEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.SendOnly();
        var instance = Endpoint.Start(configuration).GetAwaiter().GetResult();
        Session = new Session(instance);
    }

    void CreateEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.PurgeOnStartup(false);
        configuration.CustomConfigurationSource(this);

        var instance = Endpoint.Start(configuration).GetAwaiter().GetResult();
        Session = new Session(instance);
    }

    Configuration CreateConfiguration()
    {
        var configuration = new Configuration(EndpointName);
        configuration.EnableInstallers();

        configuration.ExcludeTypes(GetTypesToExclude().ToArray());

        configuration.ApplyProfiles(this);

        return configuration;
    }

    IEnumerable<Type> GetTypesToExclude()
    {
        return GetTypesToExclude(Assembly.GetAssembly(this.GetType()).GetTypes());
    }
#endif

    IEnumerable<Type> GetTypesToExclude(IEnumerable<Type> allTypes)
    {
        var allTypesToExclude = (from t in allTypes
                                 where (t.IsSubclassOf(typeof(BaseRunner)) || t.IsSubclassOf(typeof(LoopRunner))) && t != GetType()
                                 select t).ToList();

        Log.InfoFormat("This is test {0}, excluding :", GetType().Name);
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

    void InitData()
    {
        Data = new byte[(int)Permutation.MessageSize];
        new Random(0).NextBytes(Data);
    }
}