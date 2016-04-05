namespace NServiceBus5
{
    using System;
    using Categories;
    using Common;
    using NServiceBus;
    using NServiceBus.Features;
    using Utils;

    class Program
    {
        static string endpointName = "PerformanceTests_" + AppDomain.CurrentDomain.FriendlyName.Replace(' ', '_');
        static void Main(string[] args)
        {
            var permutation = PermutationParser.FromCommandlineArgs();
            var options = BusCreationOptions.Parse(args);
            var bus = CreateBus(options, permutation);
            TestRunner.EndpointName = endpointName;
            TestRunner.RunTests(bus, options);
        }

        static IBus CreateBus(BusCreationOptions options, Permutation permutation)
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

            configuration.ApplyProfiles(permutation);

            return startableBus.Start();
        }
    }
}
