//namespace SqlServerV3
//{
//    using System.Linq;
//    using NServiceBus;
//    using NServiceBus.Transports.SQLServer;
//    using TransportCompatibilityTests.Common;
//    using TransportCompatibilityTests.Common.Messages;

//    class EndpointConfigurator
//    {
//        public static BusConfiguration Configure(EndpointInfo endpointInfo, string connectionString)
//        {
//            var busConfiguration = new BusConfiguration();
//            busConfiguration.EndpointName(endpointInfo.EndpointName);

//            busConfiguration.Conventions().DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith(".Messages") && t != typeof(TestEvent));
//            busConfiguration.Conventions().DefiningEventsAs(t => t == typeof(TestEvent));

//            busConfiguration.EnableInstallers();
//            busConfiguration.UsePersistence<InMemoryPersistence>();
//            busConfiguration.UseTransport<SqlServerTransport>().ConnectionString(connectionString);

//            if (endpointInfo.DefaultSchema != null)
//            {
//                busConfiguration.UseTransport<SqlServerTransport>().DefaultSchema(endpointInfo.DefaultSchema);
//            }

//            // HINT: we should need only this instead of UseSpecificSchema but need to fix custom schema problems in reply/pub-sub in v1 and v2
//            if (endpointInfo.UseSchemaOverriderIfV3 == false)
//            {
//                endpointInfo.MessageMappings
//                    .Where(mm => mm.Schema != null).ToList()
//                    .ForEach(mm => mm.TransportAddress += "@" + mm.Schema);
//            }
//            else
//            {
//                busConfiguration.UseTransport<SqlServerTransport>().UseSpecificSchema(qn =>
//                {
//                    var mapping = endpointInfo.MessageMappings.FirstOrDefault(mm => mm.TransportAddress == qn);

//                    return mapping?.Schema;
//                });
//            }

//            busConfiguration.CustomConfigurationSource(new CustomConfiguration(endpointInfo.MessageMappings));

//            return busConfiguration;
//        }
//    }
//}
