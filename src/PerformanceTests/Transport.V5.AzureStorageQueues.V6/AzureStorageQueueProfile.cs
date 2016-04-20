using System;
using NServiceBus;
using Variables;

class AzureStorageQueueProfile : IProfile, INeedContext
{
    public IContext Context { private get; set; }

    public void Configure(BusConfiguration busConfiguration)
    {
        if ((int)MessageSize.Medium < (int)Context.Permutation.MessageSize) throw new NotSupportedException($"Message size {Context.Permutation.MessageSize} not supported by ASQ.");

        busConfiguration
            .UseTransport<AzureStorageQueueTransport>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("AzureStorageQueue"));
    }
}
