using System;
using System.Data.Common;
using NServiceBus;
using Tests.Permutations;
using Variables;

class RabbitMQProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { private get; set; }

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        var prefetchMultiplier = 1;
        var cs = ConfigurationHelper.GetConnectionString("RabbitMQ");
        var builder = new DbConnectionStringBuilder { ConnectionString = cs };
        if (builder.Remove("prefetchcount")) NServiceBus.Logging.LogManager.GetLogger(nameof(RabbitMQProfile)).Warn("Removed 'prefetchcount' value from connection string.");
        if (builder.ContainsKey("PrefetchMultiplier")) prefetchMultiplier = int.Parse((string) builder["PrefetchMultiplier"]);

        var transport = endpointConfiguration
            .UseTransport<RabbitMQTransport>()
            .PrefetchMultiplier(prefetchMultiplier);

        transport
            .ConnectionString(builder.ToString());

        if (Permutation.TransactionMode != TransactionMode.Default
            && Permutation.TransactionMode != TransactionMode.None
            && Permutation.TransactionMode != TransactionMode.Receive
            ) throw new NotSupportedException("TransactionMode: " + Permutation.TransactionMode);

        if (Permutation.TransactionMode != TransactionMode.Default)
            transport.Transactions(Permutation.GetTransactionMode());
    }
}
