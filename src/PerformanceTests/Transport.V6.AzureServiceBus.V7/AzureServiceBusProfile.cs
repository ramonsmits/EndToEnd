﻿using System;
using NServiceBus;
using NServiceBus.Logging;
using Tests.Permutations;
using Variables;

class AzureServiceBusProfile : IProfile, INeedPermutation
{
    readonly ILog Log = LogManager.GetLogger(nameof(AzureServiceBusProfile));
    readonly string connectionstring = ConfigurationHelper.GetConnectionString("AzureServiceBus");

    public Permutation Permutation { private get; set; }

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        var transport = endpointConfiguration
            .UseTransport<AzureServiceBusTransport>();

        var concurrencyLevel = ConcurrencyLevelConverter.Convert(Permutation.ConcurrencyLevel);

        transport.MessageReceivers().PrefetchCount(Permutation.PrefetchMultiplier);

        transport
            .UseTopology<ForwardingTopology>()
            .ConnectionString(connectionstring)
            .Sanitization().UseStrategy<ValidateAndHashIfNeeded>()
            ;

        transport.Queues().LockDuration(TimeSpan.FromMinutes(2));
        transport.MessagingFactories().BatchFlushInterval(TimeSpan.FromMilliseconds(100)); // Improves batched sends

        var numberOfFactoriesAndClients = 64; // Making sure that number of (receive) clients is equal to the number of factories, improves receive performance

        Log.InfoFormat("Concurrency level: {0}", concurrencyLevel);
        Log.InfoFormat("Messaging factories per namespace: {0}", numberOfFactoriesAndClients);
        Log.InfoFormat("Clients per entity: {0}", numberOfFactoriesAndClients);
        Log.InfoFormat("Prefetch count: {0}", Permutation.PrefetchMultiplier);

        transport.MessagingFactories().NumberOfMessagingFactoriesPerNamespace(numberOfFactoriesAndClients);
        transport.NumberOfClientsPerEntity(numberOfFactoriesAndClients);
        transport.Queues().EnablePartitioning(true);

        if (Permutation.TransactionMode != TransactionMode.Default
            && Permutation.TransactionMode != TransactionMode.None
            && Permutation.TransactionMode != TransactionMode.Receive
            && Permutation.TransactionMode != TransactionMode.Atomic
            ) throw new NotSupportedException("TransactionMode: " + Permutation.TransactionMode);

        if (Permutation.TransactionMode != TransactionMode.Default) transport.Transactions(Permutation.GetTransactionMode());
    }
}
