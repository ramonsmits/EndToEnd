using System;
using NServiceBus;
using NServiceBus.Settings;
using Tests.Permutations;
using Variables;

class MsmqProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { private get; set; }

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration.UseTransport<MsmqTransport>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("MSMQ"));

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
            case TransactionMode.Transactional:
                transactionSettings.EnableDistributedTransactions();
                return;
            case TransactionMode.Atomic:
                transactionSettings.DisableDistributedTransactions();
                return;
            case TransactionMode.Receive:
            default:
                throw new NotSupportedException("TransactionMode: " + Permutation.TransactionMode);
        }
    }
}
