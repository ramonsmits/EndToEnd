namespace NServiceBus6
{
    using System;
    using Common;
    using NServiceBus;
    using NServiceBus.Features;
    using Utils;

    class Program
    {
        static string endpointName = "PerformanceTests_" + AppDomain.CurrentDomain.FriendlyName.Replace(' ', '_');
        static void Main(string[] args)
        {
            var options = BusCreationOptions.Parse(args);
            var endpointInstance = CreateBus(options);
            TestRunner.EndpointName = endpointName;
            TestRunner.RunTests(endpointInstance, options);
        }

        static IEndpointInstance CreateBus(BusCreationOptions options)
        {
            if (options.Cleanup)
            {
                QueueUtils.DeleteQueuesForEndpoint(endpointName);
            }

            var configuration = new EndpointConfiguration(endpointName);
            configuration.EnableInstallers();
            configuration.DisableFeature<Audit>();
            configuration.RijndaelEncryptionService();
            configuration.LimitMessageProcessingConcurrencyTo(options.NumberOfThreads);

            switch (options.Serialization)
            {
                case SerializationKind.Xml:
                    configuration.UseSerialization<XmlSerializer>();
                    break;
                case SerializationKind.Json:
                    configuration.UseSerialization<JsonSerializer>();
                    break;
                case SerializationKind.Bson:
                case SerializationKind.Bin:
                    throw new NotSupportedException(options.Serialization.ToString());
            }

            if (options.Transport == TransportKind.Msmq)
            {
                var transport = configuration.UseTransport<MsmqTransport>();
                transport.ConnectionString("deadLetter=false;journal=false");
                if (options.SuppressDTC)
                    transport.Transactions(TransportTransactionMode.ReceiveOnly | TransportTransactionMode.SendsAtomicWithReceive);
            }

            if (options.Persistence == PersistenceKind.InMemory || options.Volatile)
            {
                configuration.DisableDurableMessages();
                configuration.UsePersistence<InMemoryPersistence>();
            }

            var endpoint = Endpoint.Create(configuration).GetAwaiter().GetResult();
            return endpoint.Start().GetAwaiter().GetResult();
        }

    }
}