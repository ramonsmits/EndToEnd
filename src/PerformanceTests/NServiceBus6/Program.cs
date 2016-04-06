namespace NServiceBus6
{
    using System;
    using System.Linq;
    using Common;
    using NServiceBus;
    using Tests.Permutations;
    using Utils;

    class Program
    {
        static string endpointName = "PerformanceTests_" + AppDomain.CurrentDomain.FriendlyName.Replace(' ', '_');
        static void Main(string[] args)
        {
            Log.Env();

            var permutation = PermutationParser.FromCommandlineArgs();
            var options = BusCreationOptions.Parse(args);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var tasks = permutation.Tests.Select(x => (IStartAndStop)assembly.CreateInstance(x)).ToArray();

            var runDuration = TimeSpan.FromMinutes(1);

            var endpointInstance = CreateBus(options, permutation);
            try
            {
                TestRunner.EndpointName = endpointName;
                TestRunner.RunTests(endpointInstance, options);
                foreach (var t in tasks) t.Start();
                System.Threading.Thread.Sleep(runDuration);
                foreach (var t in tasks) t.Stop();
            }
            finally
            {
                endpointInstance.Stop();
            }
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
