using System;
using NServiceBus;
using Variables;

class AzureStorageQueuesProfile : IProfile, INeedContext
{
    public IContext Context { private get; set; }

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        if ((int)MessageSize.Medium < (int)Context.Permutation.MessageSize) throw new NotSupportedException($"Message size {Context.Permutation.MessageSize} not supported by ASQ.");

        endpointConfiguration
            .UseTransport<AzureStorageQueueTransport>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("AzureStorageQueue"));
    }

}
