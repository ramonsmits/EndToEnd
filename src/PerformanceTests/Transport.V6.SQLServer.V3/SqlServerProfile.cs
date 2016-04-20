using NServiceBus;
using NServiceBus.Transports.SQLServer;

class SqlServerProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration
            .UseTransport<SqlServerTransport>()
            .DefaultSchema("V6")
            .ConnectionString(this.GetConnectionString("SqlServer"));
    }
}
