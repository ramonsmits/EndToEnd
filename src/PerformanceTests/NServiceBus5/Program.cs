namespace NServiceBus5
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
            var bus = CreateBus(options);
            TestRunner.EndpointName = endpointName;
            TestRunner.RunTests(bus, options);
        }

        static IBus CreateBus(BusCreationOptions options)
        {
            if (options.Cleanup)
            {
                QueueUtils.DeleteQueuesForEndpoint(endpointName);
            }

            TransportConfigOverride.MaximumConcurrencyLevel = options.NumberOfThreads;

            var configuration = new BusConfiguration();
            configuration.EndpointName(endpointName);
            configuration.EnableInstallers();
            configuration.DiscardFailedMessagesInsteadOfSendingToErrorQueue();
            configuration.DisableFeature<Audit>();
            configuration.RijndaelEncryptionService();

            switch (options.Serialization)
            {
                case SerializationKind.Xml:
                    configuration.UseSerialization<XmlSerializer>();
                    break;
                case SerializationKind.Json:
                    configuration.UseSerialization<JsonSerializer>();
                    break;
                case SerializationKind.Bson:
                    configuration.UseSerialization<BsonSerializer>();
                    break;
                case SerializationKind.Bin:
                    configuration.UseSerialization<BinarySerializer>();
                    break;
            }

            if (options.Transport == TransportKind.Msmq)
                configuration.UseTransport<MsmqTransport>().ConnectionString("deadLetter=false;journal=false");

            if (options.Persistence == PersistenceKind.InMemory || options.Volatile)
            {
                configuration.DisableDurableMessages();
                configuration.UsePersistence<InMemoryPersistence>();
            }

            if (options.SuppressDTC)
                configuration.Transactions().DisableDistributedTransactions();

            var startableBus = Bus.Create(configuration);

            // needed as a workaround for https://github.com/Particular/NServiceBus/issues/3091
            startableBus.OutgoingHeaders.Add("NServiceBus.RijndaelKeyIdentifier", "20151014");

            return startableBus.Start();
        }
    }
}