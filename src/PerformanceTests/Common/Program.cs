namespace Host
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using NServiceBus;
    using NServiceBus.Logging;
    using Tests.Permutations;
    using VisualStudioDebugHelper;

    class Program
    {
        static ILog Log;

        static string endpointName = "PerformanceTest";

        static int Main()
        {
            LogManager.Use<NLogFactory>();
            NLog.LogManager.Configuration.DefaultCultureInfo = CultureInfo.InvariantCulture;

            Log = LogManager.GetLogger(typeof(Program));

            DebugAttacher.AttachDebuggerToVisualStudioProcessFromCommandLineParameter();

            AppDomain.CurrentDomain.FirstChanceException += (o, ea) => { Log.Debug("FirstChanceException", ea.Exception); };
            AppDomain.CurrentDomain.UnhandledException += (o, ea) => { Log.Error("UnhandledException", ea.ExceptionObject as Exception); };

            try
            {
                var permutation = PermutationParser.FromCommandlineArgs();
                LogPermutation(permutation);

                InvokeSetupImplementations(permutation);

                using (Statistics.Initialize(permutation))
                {
                    EnvironmentStats.Write();

                    ValidateServicePointManager(permutation);

                    if (Environment.UserInteractive) Console.Title = PermutationParser.ToFriendlyString(permutation);

                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    var runnableTest = permutation.Tests.Select(x => (BaseRunner)assembly.CreateInstance(x)).Single();

                    Log.InfoFormat("Executing scenario: {0}", runnableTest);
                    runnableTest.Execute(permutation, endpointName)
                        .ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            catch (NotSupportedException nsex)
            {
                Log.Warn("Not supported", nsex);
                return (int)ReturnCodes.NotSupported;
            }
            catch (Exception ex)
            {
                Log.Fatal("Main", ex);
                NLog.LogManager.Shutdown();
                throw;
            }
            return (int)ReturnCodes.OK;
        }

        static void InvokeSetupImplementations(Permutation permutation)
        {
            foreach (var instance in AssemblyScanner.GetAll<ISetup>())
            {
                var p = instance as INeedPermutation;
                if (p != null) p.Permutation = permutation;
                Log.InfoFormat("Invoke setup: {0}", instance);
                instance.Setup();
            }
        }

        static void LogPermutation(Permutation permutation)
        {
            var log = LogManager.GetLogger("Permutation");
            log.InfoFormat("Category: {0}", permutation.Category);
            log.InfoFormat("Description: {0}", permutation.Description);
            log.InfoFormat("Fixture: {0}", permutation.Fixture);
            log.InfoFormat("Code: {0}", permutation.Code);
            log.InfoFormat("Id: {0}", permutation.Id);
        }

        static void ValidateServicePointManager(Permutation permutation)
        {
            var value = ConcurrencyLevelConverter.Convert(permutation.ConcurrencyLevel);
            if (ServicePointManager.DefaultConnectionLimit < value)
            {
                Log.WarnFormat("ServicePointManager.DefaultConnectionLimit value {0} is lower then maximum concurrency limit of {1} this can limit performance.",
                    ServicePointManager.DefaultConnectionLimit,
                    value
                    );
            }

            if (ServicePointManager.Expect100Continue)
            {
                Log.WarnFormat("ServicePointManager.Expect100Continue is set to True, consider setting this value to False to increase PUT operations.");
            }

            if (ServicePointManager.UseNagleAlgorithm)
            {
                Log.WarnFormat("ServicePointManager.UseNagleAlgorithm is set to True, consider setting this value to False to decrease Latency.");
            }
        }
    }
}
