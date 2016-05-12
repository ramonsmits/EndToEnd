#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using NServiceBus.Logging;
using Tests.Permutations;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Common.Scenarios;
using Variables;

public abstract class BaseRunner : IConfigurationSource, IContext
{
    readonly ILog Log = LogManager.GetLogger("BaseRunner");

    public Permutation Permutation { get; private set; }
    public string EndpointName { get; private set; }
    ISession Session { get; set; }

    protected byte[] Data { private set; get; }
    protected bool SendOnly { get; set; }

    public async Task Execute(Permutation permutation, string endpointName)
    {
        Permutation = permutation;
        EndpointName = endpointName;

        InitData();

        await CreateSeedData();
        await CreateEndpoint();

        try
        {
            await Start(Session);

            if (!IsSeedingData)
            {
                Log.InfoFormat("Warmup: {0}", Settings.WarmupDuration);
                await Task.Delay(Settings.WarmupDuration).ConfigureAwait(false);
            }

            var runDuration = IsSeedingData
                ? Settings.RunDuration - Settings.SeedDuration
                : Settings.RunDuration;

            Log.InfoFormat("Run: {0}", runDuration);

            Statistics.Instance.Reset(GetType().Name);
            await Task.Delay(runDuration).ConfigureAwait(false);
            Statistics.Instance.Dump();

            await Stop();
        }
        finally
        {
            await Session.Close();
        }
    }

    protected virtual Task Start(ISession session)
    {
        return Task.FromResult(0);
    }

    protected virtual Task Stop()
    {
        return Task.FromResult(0);
;    }

    async Task CreateSeedData()
    {
        var seedCreator = this as ICreateSeedData;
        if (seedCreator == null) return;

        await CreateOrPurgeQueues();
        await CreateSendOnlyEndpoint();

        try
        {
            Log.InfoFormat("Start seeding messages for {0} seconds...", Settings.SeedDuration.TotalSeconds);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(Settings.SeedDuration);

            var maxConcurrency = ConcurrencyLevelConverter.Convert(Permutation.ConcurrencyLevel);

            var instance = (ICreateSeedData)this;
            var count = 0L;
            var start = Stopwatch.StartNew();

            var po = new ParallelOptions
            {
                MaxDegreeOfParallelism = maxConcurrency
            };

            Parallel.ForEach(IterateUntilFalse(() => !cts.Token.IsCancellationRequested), po, b =>
            {
                instance.SendMessage(Session).ConfigureAwait(false).GetAwaiter().GetResult();
                Interlocked.Increment(ref count);
            });


            //var tasks = new List<Task>();
            //while (!cts.IsCancellationRequested)
            //{
            //    tasks.Add(instance.SendMessage(Session));
            //}

            //foreach (var t in tasks)
            //    await Task.WhenAll(tasks);

            var elapsed = start.Elapsed;
            var avg = count / elapsed.TotalSeconds;
            Log.InfoFormat("Done seeding, seeded {0:N0} messages, {1:N1} msg/s", count, avg);
            LogManager.GetLogger("Statistics").InfoFormat("{0}: {1:0.0} ({2})", "SeedThroughputAvg", avg, "msg/s");
            LogManager.GetLogger("Statistics").InfoFormat("{0}: {1:0.0} ({2})", "SeedCount", count, "#");
            LogManager.GetLogger("Statistics").InfoFormat("{0}: {1:0.0} ({2})", "SeedDuration", elapsed.TotalMilliseconds, "ms");
        }
        finally
        {
            await Session.Close();
        }
    }

#if Version5
    Task CreateOrPurgeQueues()
    {
        var configuration = CreateConfiguration();
        if (IsPurgingSupported) configuration.PurgeOnStartup(true);
        using (Bus.Create(configuration).Start()) { }
        return Task.FromResult(0);
    }

    Task CreateSendOnlyEndpoint()
    {
        var configuration = CreateConfiguration();
        var instance = Bus.CreateSendOnly(configuration);
        Session = new Session(instance);
        return Task.FromResult(0);
    }

    Task CreateEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.CustomConfigurationSource(this);

        if (SendOnly)
        {
            Session = new Session(Bus.CreateSendOnly(configuration));
            return Task.FromResult(0);
        }

        configuration.PurgeOnStartup(!IsSeedingData && IsPurgingSupported);
        Session = new Session(Bus.Create(configuration).Start());
        return Task.FromResult(0);
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
    async Task CreateOrPurgeQueues()
    {
        var configuration = CreateConfiguration();
        if (IsPurgingSupported) configuration.PurgeOnStartup(true);
        var instance = await Endpoint.Start(configuration).ConfigureAwait(false);
        await instance.Stop().ConfigureAwait(false);
    }

    async Task CreateSendOnlyEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.SendOnly();
        var instance = await Endpoint.Start(configuration).ConfigureAwait(false);
        Session = new Session(instance);
    }

    async Task CreateEndpoint()
    {
        var configuration = CreateConfiguration();
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.CustomConfigurationSource(this);

        if (SendOnly)
        {
            configuration.SendOnly();
            Session = new Session(await Endpoint.Start(configuration).ConfigureAwait(false));
            return;
        }

        configuration.PurgeOnStartup(!IsSeedingData && IsPurgingSupported);

        var instance = Endpoint.Start(configuration).ConfigureAwait(false).GetAwaiter().GetResult();
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

        //read from existing config 
        var config = (UnicastBusConfig)ConfigurationManager.GetSection(typeof(UnicastBusConfig).Name);
        if (config != null) throw new InvalidOperationException("UnicastBUs Configuration should be in code using IConfigureUnicastBus interface.");

        if (typeof(T) == typeof(UnicastBusConfig) && null != (configureUnicastBus = this as IConfigureUnicastBus))
        {
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

    bool IsSeedingData => this is ICreateSeedData;
    bool IsPurgingSupported => Permutation.Transport != Transport.AzureServiceBus;

    static IEnumerable<int> IterateUntilFalse(Func<bool> condition)
    {
        var i = 0;
        while (condition()) yield return i++;
    }
}
