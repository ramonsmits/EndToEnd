namespace NServiceBus5
{
    using System;
    using Categories;
    using Common;
    using NServiceBus;
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

            var configuration = new BusConfiguration();
            configuration.EndpointName(endpointName);
            configuration.EnableInstallers();
            configuration.DiscardFailedMessagesInsteadOfSendingToErrorQueue();

            var startableBus = Bus.Create(configuration);

            configuration.ApplyProfiles(permutation);

            return startableBus.Start();
        }
    }
}
