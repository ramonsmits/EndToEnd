using NServiceBus;

namespace Version_6_2
{
    class Program
    {
        static string endpointName = "PersistenceTest6_2";

        static void Main()
        {
            var bus = CreateBus();
            TestRunner.EndpointName = endpointName;
            TestRunner.RunTests(bus);
        }

        static IBus CreateBus()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.EndpointName(endpointName);
            busConfiguration.Conventions().ApplyMessageConventions();
            busConfiguration.UseSerialization<JsonSerializer>();
            busConfiguration.UseTransport<MsmqTransport>();
            busConfiguration.UsePersistence<NHibernatePersistence>();
            busConfiguration.EnableInstallers();
            var startableBus = Bus.Create(busConfiguration);
            return startableBus.Start();
        }
    }
}