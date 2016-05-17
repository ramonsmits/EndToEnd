using System.Collections.Generic;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NServiceBus.SagaPersisters.NHibernate.AutoPersistence;

namespace Version_5_0
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
                    {"dialect", dialect},
                    {NHibernate.Cfg.Environment.ConnectionString, connectionString}
                });

            var modelMapper = new SagaModelMapper(new[] { typeof(TSagaData) });

            configuration.AddMapping(modelMapper.Compile());
            SessionFactory = configuration.BuildSessionFactory();
            new SchemaUpdate(configuration).Execute(false, true);
        }

        public ISessionFactory SessionFactory { get; private set; }
    }
}