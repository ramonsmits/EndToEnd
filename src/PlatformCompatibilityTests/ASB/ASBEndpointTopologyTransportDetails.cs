using System.Configuration;
using NServiceBus;
using NServiceBus.AzureServiceBus;

namespace ServiceControlCompatibilityTests
{
    public class ASBEndpointTopologyTransportDetails : ITransportDetails
    {
        const string TransportTypeName = "NServiceBus.AzureServiceBusTransport, NServiceBus.Azure.Transports.WindowsAzureServiceBus";

        public ASBEndpointTopologyTransportDetails(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string TransportName => "AzureServiceBus";

        public void ApplyTo(Configuration configuration)
        {
            configuration.ConnectionStrings.ConnectionStrings.Set("NServiceBus/Transport", connectionString);
            var settings = configuration.AppSettings.Settings;
            settings.Set(SettingsList.TransportType, TransportTypeName);
        }

        public void ConfigureEndpoint(EndpointConfiguration endpointConfig)
        {
            endpointConfig.UseTransport<AzureServiceBusTransport>()
                .ConnectionString(connectionString)
                .UseTopology<EndpointOrientedTopology>();
        }

        string connectionString;
    }
}