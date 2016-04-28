using NServiceBus;

class AzureServiceBusProfile : IProfile
{
    readonly string connectionstring = ConfigurationHelper.GetConnectionString("AzureServiceBus");

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .ConnectionString(connectionstring);
    }
}
