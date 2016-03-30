namespace NServiceBus3
{
    using System;
    using System.Threading;
    using Common;
    using NServiceBus;
    using NServiceBus.Installation.Environments;
    using NServiceBus.Unicast;
    using Utils;

    class Program
    {
        static string endpointName = "PerformanceTests_" + AppDomain.CurrentDomain.FriendlyName.Replace(' ', '_');

        static void Main(string[] args)
        {
            //HACK: for trial dialog issue https://github.com/Particular/NServiceBus/issues/2001
            var synchronizationContext = SynchronizationContext.Current;
            var options = BusCreationOptions.Parse(args);
            var bus = CreateBus(options);
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            TestRunner.EndpointName = endpointName;

            TestRunner.RunTests(bus, options);
        }

        static UnicastBus CreateBus(BusCreationOptions options)
        {
            if (options.Cleanup)
            {
                QueueUtils.DeleteQueuesForEndpoint(endpointName);
            }

            var configure = Configure.With(AllAssemblies.Except("NServiceBus3.vshost.exe"))
                .DefineEndpointName(endpointName)
                .DefaultBuilder();

            configure.RijndaelEncryptionService();

            switch (options.Serialization)
            {
                case SerializationKind.Xml:
                    configure.XmlSerializer();
                    break;
                case SerializationKind.Json:
                    configure.JsonSerializer();
                    break;
                case SerializationKind.Bson:
                    configure.BsonSerializer();
                    break;
                case SerializationKind.Bin:
                    configure.BinarySerializer();
                    break;
            }

            if (options.Saga)
                configure.Sagas();

            if (options.Transport == TransportKind.Msmq)
                configure.MsmqTransport();

            if (options.Persistence == PersistenceKind.InMemory || options.Volatile)
            {
                configure.InMemorySagaPersister();
                configure.UseInMemoryTimeoutPersister();
                configure.InMemorySubscriptionStorage();
            }

            configure.RunTimeoutManager();

            var startableBus = configure.InMemoryFaultManagement().UnicastBus().CreateBus();

            // needed as a workaround for https://github.com/Particular/NServiceBus/issues/3091
            var outgoingHeaders = ((IBus)startableBus).OutgoingHeaders;
            outgoingHeaders.Add("NServiceBus.RijndaelKeyIdentifier", "20151014");

            return (UnicastBus)startableBus.Start(
                () => Configure.Instance.ForInstallationOn<Windows>().Install());
        }
    }
}
