using System.Data.Common;
using NServiceBus;

class RabbitMQProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        var cs = ConfigurationHelper.GetConnectionString("RabbitMQ");
        var builder = new DbConnectionStringBuilder { ConnectionString = cs };
        if (builder.Remove("prefetchcount")) NServiceBus.Logging.LogManager.GetLogger(nameof(RabbitMQProfile)).Warn("Removed 'prefetchcount' value from connection string.");

        endpointConfiguration
            .UseTransport<RabbitMQTransport>()
            .ConnectionString(builder.ToString());
    }
}
