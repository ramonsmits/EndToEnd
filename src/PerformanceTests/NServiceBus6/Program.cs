namespace NServiceBus6
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
            var endpointInstance = CreateBus(options, permutation);
            TestRunner.EndpointName = endpointName;
            TestRunner.RunTests(endpointInstance, options);
        }

        static IEndpointInstance CreateBus(BusCreationOptions options, Permutation permutation)
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

            configuration.ApplyProfiles(permutation);

            var endpoint = Endpoint.Create(configuration).GetAwaiter().GetResult();
            return endpoint.Start().GetAwaiter().GetResult();
        }

    }
}
