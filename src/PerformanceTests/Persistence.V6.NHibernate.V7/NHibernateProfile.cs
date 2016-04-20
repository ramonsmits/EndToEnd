using System.Reflection;
using NServiceBus;
using NServiceBus.Persistence;

class NHibernateProfile : IProfile, ISetup
{
    public void Configure(EndpointConfiguration cfg)
    {
        cfg
            .UsePersistence<NHibernatePersistence>()
            .ConnectionString(ConfigurationHelper.GetConnectionString("NHibernate"))
            //.EnableCachingForSubscriptionStorage(TimeSpan.FromSeconds(5))
            ;
    }

    void ISetup.Setup()
    {
        var cs = ConfigurationHelper.GetConnectionString("NHibernate");
        var sql = Assembly.GetExecutingAssembly().GetManifestResourceText("Persistence.V6.NHibernate.init.sql");
        SqlHelper.ExecuteScript(cs, sql);
    }
}
