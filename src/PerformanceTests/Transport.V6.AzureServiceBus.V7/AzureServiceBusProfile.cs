using NServiceBus;
using NServiceBus.AzureServiceBus;

class AzureServiceBusProfile : IProfile
{
    readonly string connectionstring = ConfigurationHelper.GetConnectionString("AzureServiceBus");

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .UseTopology<ForwardingTopology>()
            .ConnectionString(connectionstring);
    }
}
