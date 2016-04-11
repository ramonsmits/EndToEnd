using NServiceBus;
using Common;

class AzureServiceBusProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .ConnectionString(this.GetConnectionString("AzureServiceBus"));

        endpointConfiguration.PurgeOnStartup(false);
    }
}
