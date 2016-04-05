namespace NServiceBus5
{
    using System;
    using System.Linq;
    using Common;
    using Common.Saga;
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

            using (var bus = CreateBus(options, permutation))
            {
                TestRunner.EndpointName = endpointName;
                TestRunner.RunTests(bus, options);
                foreach (var t in tasks) t.Start();
                System.Threading.Thread.Sleep(runDuration);
                foreach (var t in tasks) t.Stop();
            }
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
