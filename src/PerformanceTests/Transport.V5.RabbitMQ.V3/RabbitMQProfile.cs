using System;
using NServiceBus;
using NServiceBus.Settings;
using Tests.Permutations;
using Variables;

class RabbitMQProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { private get; set; }

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration
            .UseTransport<RabbitMQTransport>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("RabbitMQ"));

        InitTransactionMode(busConfiguration.Transactions());
    }

    void InitTransactionMode(TransactionSettings transactionSettings)
    {
        switch (Permutation.TransactionMode)
        {
            case TransactionMode.Default:
                return;
            case TransactionMode.None:
                transactionSettings.Disable();
                return;
            case TransactionMode.Receive:
                transactionSettings.DisableDistributedTransactions();
                return;
            case TransactionMode.Atomic:
            case TransactionMode.Transactional:
            default:
                throw new NotSupportedException("TransactionMode: " + Permutation.TransactionMode);
        }
    }
}
