using System.Configuration;
using NServiceBus;

namespace ServiceControlCompatibilityTests
{
    public class SqlTransportDetails : ITransportDetails
    {
        const string TransportTypeName = "NServiceBus.SqlServerTransport, NServiceBus.Transports.SQLServer";

        public SqlTransportDetails(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public string TransportName => "SQLServer";

        public void ApplyTo(Configuration configuration)
        {
            configuration.ConnectionStrings.ConnectionStrings.Set("NServiceBus/Transport", connectionString);
            var settings = configuration.AppSettings.Settings;
            settings.Set(SettingsList.TransportType, TransportTypeName);
        }

        public void ConfigureEndpoint(EndpointConfiguration endpointConfig)
        {
            endpointConfig.UseTransport<SqlServerTransport>()
                .ConnectionString(connectionString);
        }

        string connectionString;
    }
}