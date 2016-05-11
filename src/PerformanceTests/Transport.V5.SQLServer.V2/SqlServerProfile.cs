using System.Reflection;
using NServiceBus;
using Tests.Permutations;

class SqlServerProfile : IProfile, ISetup, INeedPermutation
{
    public Permutation Permutation { private get; set; }

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration
            .UseTransport<SqlServerTransport>()
            .DefaultSchema("V5")
            .ConnectionString(ConfigurationHelper.GetConnectionString(Permutation.Transport.ToString()));
    }

    void ISetup.Setup()
    {
        var cs = ConfigurationHelper.GetConnectionString(Permutation.Transport.ToString());
        var sql = Assembly.GetExecutingAssembly().GetManifestResourceText("Transport.V5.SQLServer.init.sql");
        SqlHelper.CreateDatabase(cs);
        SqlHelper.ExecuteScript(cs, sql);
    }
}
