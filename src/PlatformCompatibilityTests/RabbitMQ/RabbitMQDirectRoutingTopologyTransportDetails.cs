namespace ServiceControlCompatibilityTests
{
    using System.Configuration;
    using System.Threading.Tasks;
    using NServiceBus;

    public class RabbitMQDirectRoutingTopologyTransportDetails : ITransportDetails
    {
        const string TransportTypeName = "NServiceBus.RabbitMQTransport, NServiceBus.Transports.RabbitMQ";

        public RabbitMQDirectRoutingTopologyTransportDetails(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string TransportName => "RabbitMQ";

        public Task Initialize()
        {
            return Task.FromResult(0);
        }

        public void ApplyTo(Configuration configuration)
        {
            configuration.ConnectionStrings.ConnectionStrings.Set("NServiceBus/Transport", connectionString);
            var settings = configuration.AppSettings.Settings;
            settings.Set(SettingsList.TransportType, TransportTypeName);
        }

        public void ConfigureEndpoint(string endpointName, EndpointConfiguration endpointConfig)
        {
            endpointConfig.UseTransport<RabbitMQTransport>()
                .UseDirectRoutingTopology()
                .ConnectionString(connectionString)
                ;

            endpointConfig.PurgeOnStartup(true);
        }

        string connectionString;
    }
}