using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NServiceBus.Features;
using NServiceBus.SagaPersisters.NHibernate.AutoPersistence;
using NServiceBus.Sagas;
using NServiceBus.Settings;
using Environment = NHibernate.Cfg.Environment;


namespace Version_7_0
{
    public class NHibernateSessionFactory<TSagaData>
    {
        const string dialect = "NHibernate.Dialect.MsSql2012Dialect";

        public void Init()
        {
            var connectionString = @"Data Source=SCHMETTERLING\SQLEXPRESS;Initial Catalog=persistencetests;Integrated Security=True"; //ConfigurationManager.ConnectionStrings[0].ConnectionString;

            var configuration = new Configuration()
                .AddProperties(new Dictionary<string, string>
                {
                        {"dialect", dialect},
                        {Environment.ConnectionString, connectionString}
                });

            var metaModel = new SagaMetadataCollection();
            metaModel.Initialize(new[] { typeof(TestSaga) });
            var metadata = metaModel.Find(typeof(TestSaga));
            var mapper = new SagaModelMapper(metaModel, new[] { metadata.SagaEntityType });
            configuration.AddMapping(mapper.Compile());
            
            SessionFactory = configuration.BuildSessionFactory();
            new SchemaUpdate(configuration).Execute(false, true);
        }

        public ISessionFactory SessionFactory { get; private set; }
    }
}