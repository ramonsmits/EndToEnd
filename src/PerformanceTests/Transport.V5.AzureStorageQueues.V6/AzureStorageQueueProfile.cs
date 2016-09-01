using System;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.Settings;
using Variables;

class AzureStorageQueueProfile : IProfile, INeedContext
{
    readonly ILog Log = LogManager.GetLogger<AzureStorageQueueProfile>();

    public IContext Context { private get; set; }

    public void Configure(BusConfiguration busConfiguration)
    {
        if ((int)MessageSize.Medium < (int)Context.Permutation.MessageSize) throw new NotSupportedException($"Message size {Context.Permutation.MessageSize} not supported by ASQ.");

        busConfiguration
            .UseTransport<AzureStorageQueueTransport>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("AzureStorageQueue"));

        InitTransactionMode(busConfiguration.Transactions());
    }

    void InitTransactionMode(TransactionSettings transactionSettings)
    {
        var mode = Context.Permutation.TransactionMode;
        switch (mode)
        {
            case TransactionMode.Default:
                return;
            case TransactionMode.None:
                transactionSettings.Disable();
                return;
            case TransactionMode.Receive:
                transactionSettings.DisableDistributedTransactions();
                return;
            case TransactionMode.Atomic: // Can mimic batched sends behavior using transaction scope enlistment. Results in transmitting message after all processing completes.
                transactionSettings.EnableDistributedTransactions();
                Log.WarnFormat("Using TransactionMode.Atomic results in batched sends but not actual atomic receive with sends.");
                return;
            case TransactionMode.Transactional:
            default:
                throw new NotSupportedException("TransactionMode: " + mode);
        }
    }
}
