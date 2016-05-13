using System.Collections.Generic;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NServiceBus.SagaPersisters.NHibernate.AutoPersistence;

namespace Version_6_2
{
    public class NHibernateSessionFactory<TSagaData>
    {
        const string dialect = "NHibernate.Dialect.MsSql2012Dialect";

        public void Init()
        {
            var connectionString = "Data Source=.;Initial Catalog=persistencetests; Trusted_Connection=True;";//ConfigurationManager.ConnectionStrings[0].ConnectionString;

            var configuration = new global::NHibernate.Cfg.Configuration()
                .AddProperties(new Dictionary<string, string>
                {
                    { "dialect", dialect },
                    { global::NHibernate.Cfg.Environment.ConnectionString, connectionString }
                });

            var modelMapper = new SagaModelMapper(new[] { typeof(TSagaData) });

            configuration.AddMapping(modelMapper.Compile());
            SessionFactory = configuration.BuildSessionFactory();
            new SchemaUpdate(configuration).Execute(false, true);
        }

        public ISessionFactory SessionFactory { get; private set; }
    }
}