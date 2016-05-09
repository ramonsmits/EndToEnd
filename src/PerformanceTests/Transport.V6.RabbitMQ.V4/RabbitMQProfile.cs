using NServiceBus;

class RabbitMQProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration
            .UseTransport<RabbitMQTransport>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("RabbitMQ"));
    }
}
