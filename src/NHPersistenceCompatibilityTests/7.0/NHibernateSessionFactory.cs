using System.Collections.Generic;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NServiceBus.Features;
using NServiceBus.SagaPersisters.NHibernate.AutoPersistence;
using NServiceBus.Sagas;
using NServiceBus.Settings;

namespace Version_7_0
{
    public class NHibernateSessionFactory<TSagaData>
    {
        const string dialect = "NHibernate.Dialect.MsSql2012Dialect";

        public void Init()
        {
            var connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=persistencetests;Integrated Security=True";//ConfigurationManager.ConnectionStrings[0].ConnectionString;

            var configuration = new NHibernate.Cfg.Configuration()
                .AddProperties(new Dictionary<string, string>
                {
                    { "dialect", dialect },
                    { NHibernate.Cfg.Environment.ConnectionString, connectionString }
                });

            var metaModel = new SagaMetadataCollection();
            var builder = new NHibernateSagaStorage();
            var settings = new SettingsHolder();
            var types = new[] {typeof(TSagaData)};

            metaModel.Initialize(types);
            settings.Set<SagaMetadataCollection>(metaModel);
            settings.Set("TypesToScan", types);
            builder.ApplyMappings(settings, configuration);

            SessionFactory = configuration.BuildSessionFactory();
            new SchemaUpdate(configuration).Execute(false, true);
        }

        public ISessionFactory SessionFactory { get; private set; }
    }
}