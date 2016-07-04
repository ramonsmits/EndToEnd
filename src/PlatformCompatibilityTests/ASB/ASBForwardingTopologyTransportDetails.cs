﻿using System.Configuration;
using NServiceBus;
using NServiceBus.AzureServiceBus;

namespace ServiceControlCompatibilityTests
{
    using System.Threading.Tasks;

    public class ASBForwardingTopologyTransportDetails : ITransportDetails
    {
        const string TransportTypeName = "NServiceBus.AzureServiceBusTransport, NServiceBus.Azure.Transports.WindowsAzureServiceBus";

        public ASBForwardingTopologyTransportDetails(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string TransportName => "AzureServiceBus";

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
            endpointConfig.UseTransport<AzureServiceBusTransport>()
                .ConnectionString(connectionString)
                .UseTopology<ForwardingTopology>();
        }


        string connectionString;
    }
}