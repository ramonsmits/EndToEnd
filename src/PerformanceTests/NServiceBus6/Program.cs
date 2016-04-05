namespace NServiceBus6
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
            configuration.LimitMessageProcessingConcurrencyTo(options.NumberOfThreads);

            configuration.ApplyProfiles(permutation);

            var endpoint = Endpoint.Create(configuration).GetAwaiter().GetResult();
            return endpoint.Start().GetAwaiter().GetResult();
        }
    }
}
