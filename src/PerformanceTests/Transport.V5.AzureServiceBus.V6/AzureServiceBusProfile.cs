using NServiceBus;

class AzureServiceBusProfile : IProfile
{
    readonly string connectionstring = ConfigurationHelper.GetConnectionString("AzureServiceBus");

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration.ScaleOut().UseSingleBrokerQueue();

        busConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .ConnectionString(connectionstring);

    }
}
