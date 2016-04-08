namespace Host
{
    using System.Threading.Tasks;
    using Common;
    using NServiceBus;
    using Tests.Permutations;
    using Utils;

    partial class Program
    {
        public static IEndpointInstance Instance;

        static void Run(BusCreationOptions options, Permutation permutation, IStartAndStop[] tasks)
        {
            Instance = CreateBus(options, permutation).Result;
            try
            {
                foreach (var t in tasks) t.Start();
                System.Threading.Thread.Sleep(Settings.WarmupDuration);
                Statistics.Instance.Reset();
                System.Threading.Thread.Sleep(Settings.RunDuration);
                Statistics.Instance.Dump();
                foreach (var t in tasks) t.Stop();
            }
            finally
            {
                Instance.Stop();
            }
        }

        static async Task<IEndpointInstance> CreateBus(BusCreationOptions options, Permutation permutation)
        {
            if (options.Cleanup)
            {
                QueueUtils.DeleteQueuesForEndpoint(endpointName);
            }

            var configuration = new EndpointConfiguration(endpointName);
            configuration.EnableInstallers();
            configuration.LimitMessageProcessingConcurrencyTo(options.NumberOfThreads);
            configuration.ApplyProfiles(permutation);
            configuration.EnableFeature<NServiceBus.Performance.SimpleStatisticsFeature>();

            var endpoint = await Endpoint.Create(configuration);
            return await endpoint.Start();
        }
    }
}
