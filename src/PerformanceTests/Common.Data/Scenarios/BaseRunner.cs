﻿
using System;
using System.Threading.Tasks;
using Common.Scenarios;
#if Version6
    using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif

using NServiceBus;
using NServiceBus.Logging;
using Tests.Permutations;
using Utils;

public abstract class BaseRunner
{
    readonly ILog Log = LogManager.GetLogger("BaseRunner");

#if Version5
    protected IBus EndpointInstance { get; set; }
#else
    protected IEndpointInstance EndpointInstance { get; set; }
#endif

    public virtual void Execute(Permutation permutation, BusCreationOptions options, string endpointName)
    {
        if (this is ICreateSeedData) CreateSeedData(permutation, endpointName);

        EndpointInstance = CreateEndpoint(permutation, options, endpointName);

        Start();
        Log.InfoFormat("Warmup: {0}", Settings.WarmupDuration);
        System.Threading.Thread.Sleep(Settings.WarmupDuration);
        Statistics.Instance.Reset();
        Log.InfoFormat("Run: {0}", Settings.RunDuration);
        System.Threading.Thread.Sleep(Settings.RunDuration);
        Statistics.Instance.Dump();
        Stop();
    }
    protected abstract void Start();
    protected abstract void Stop();

    private void CreateSeedData(Permutation permutation, string endpointName)
    {
        var seedCreator = ((ICreateSeedData) this);
        if (seedCreator.SeedSize == 0) throw new InvalidOperationException("SeedSize was not set.");

        var configuration = CreateConfiguration(permutation, endpointName);
        CreateQueues(configuration);

        configuration = CreateConfiguration(permutation, endpointName);
        var endpoint = CreateSendOnlyEndpoint(configuration);

        Parallel.For(0, seedCreator.SeedSize, ((i, state) =>
        {
            if (i % 5000 == 0)
                Log.Info($"Seeded {i} messages.");

            ((ICreateSeedData)this).SendMessage(endpoint, endpointName);
        }));
        Log.Info($"Seeded total of {seedCreator.SeedSize} messages.");
    }

#if Version5
    void CreateQueues(Configuration configuration)
    {
        configuration.PurgeOnStartup(false);
        var createQueuesBus = Bus.Create(configuration).Start();
        createQueuesBus.Dispose();
    }

    ISendOnlyBus CreateSendOnlyEndpoint(Configuration configuration)
    {
        return Bus.CreateSendOnly(configuration);
    }

    IBus CreateEndpoint(Permutation permutation, BusCreationOptions options, string endpointName)
    {
        var configuration = CreateConfiguration(permutation, endpointName);
        configuration.PurgeOnStartup(false);
        return Bus.Create(configuration).Start();
    }

    BusConfiguration CreateConfiguration(Permutation permutation, string endpointName)
    {
        var configuration = new BusConfiguration();
        configuration.EndpointName(endpointName);
        configuration.EnableInstallers();
        configuration.DiscardFailedMessagesInsteadOfSendingToErrorQueue();
        configuration.ApplyProfiles(permutation);

        return configuration;
    }
#else
    void CreateQueues(Configuration configuration)
    {
        configuration.PurgeOnStartup(true);
        Endpoint.Create(configuration).GetAwaiter().GetResult();
    }

    IEndpointInstance CreateSendOnlyEndpoint(Configuration configuration)
    {
        configuration.SendOnly();
        return Endpoint.Start(configuration).GetAwaiter().GetResult();
    }

    IEndpointInstance CreateEndpoint(Permutation permutation, BusCreationOptions options, string endpointName)
    {
        var configuration = CreateConfiguration(permutation, endpointName);
        configuration.LimitMessageProcessingConcurrencyTo(options.NumberOfThreads);
        configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();
        configuration.PurgeOnStartup(false);

        return Endpoint.Start(configuration).GetAwaiter().GetResult();
    }

    EndpointConfiguration CreateConfiguration(Permutation permutation, string endpointName)
    {
        var configuration = new EndpointConfiguration(endpointName);
        configuration.EnableInstallers();
        configuration.ApplyProfiles(permutation);

        return configuration;
    }
#endif
}