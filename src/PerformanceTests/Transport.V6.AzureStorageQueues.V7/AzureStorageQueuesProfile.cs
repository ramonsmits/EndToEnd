using NServiceBus;

class AzureStorageQueuesProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
    }
}
