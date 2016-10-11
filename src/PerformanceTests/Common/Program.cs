namespace Host
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Microsoft.Win32;
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

            InitAppDomainEventLogging();
            CheckPowerPlan();
            CheckIfWindowsDefenderIsRunning();
            CheckProcessorScheduling();

            InitBatchHelper();

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

                    PostChecks();
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

        static void InitBatchHelper()
        {
#if Version5
            BatchHelper.Instance = new BatchHelper.ParallelFor();
#endif

#if Version6
            BatchHelper.Instance = new BatchHelper.TaskWhenAll();
#endif
        }

        static void CheckIfWindowsDefenderIsRunning()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows Defender\Real-Time Protection");

            if (key != null && 0 == (int)key.GetValue("DisableRealtimeMonitoring", 1))
            {
                Log.Warn("Windows Defender is running, consider disabling real-time protection!");
                Thread.Sleep(3000);
            }
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

            log.InfoFormat("AuditMode: {0}", permutation.AuditMode);
            log.InfoFormat("MessageSize: {0}", permutation.MessageSize);
            log.InfoFormat("Version: {0}", permutation.Version);
            log.InfoFormat("Outbox: {0}", permutation.OutboxMode);
            log.InfoFormat("Persistence: {0}", permutation.Persister);
            log.InfoFormat("Platform: {0}", permutation.Platform);
            log.InfoFormat("Serializer: {0}", permutation.Serializer);
            log.InfoFormat("Transport: {0}", permutation.Transport);
            log.InfoFormat("GC: {0}", permutation.GarbageCollector);
            log.InfoFormat("TransactionMode: {0}", permutation.TransactionMode);
            log.InfoFormat("ConcurrencyLevel: {0}", permutation.ConcurrencyLevel);
            log.InfoFormat("ScaleOut: {0}", permutation.ScaleOut);
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

        static void PostChecks()
        {
            if (Statistics.Instance.NumberOfMessages == 0)
            {
                Log.Error("NumberOfMessages equals 0, expected atleast one message to be processed.");
            }

            if (Statistics.Instance.NumberOfRetries > Statistics.Instance.NumberOfMessages)
            {
                Log.Error("NumberOfRetries is great than NumberOfMessages, too many errors occured during processing.");
            }
        }

        static void CheckPowerPlan()
        {
            try
            {
                var highperformance = new Guid("8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
                var id = Powerplan.GetActive();

                Log.InfoFormat("Powerplan: {0}", id);

                if (id != highperformance)
                {
                    Log.WarnFormat("Power option not set to High Performance, consider setting it to high performance!");
                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Powerplan check failed, ignoring", ex);
            }
        }

        static void InitAppDomainEventLogging()
        {
            var firstChanceLog = LogManager.GetLogger("FirstChanceException");
            var unhandledLog = LogManager.GetLogger("UnhandledException");
            var domain = AppDomain.CurrentDomain;

            domain.FirstChanceException += (o, ea) => { firstChanceLog.Debug(ea.Exception.Message, ea.Exception); };
            domain.UnhandledException += (o, ea) =>
            {
                var exception = ea.ExceptionObject as Exception;
                if (exception != null) unhandledLog.Error(exception.Message, exception);
            };
        }

        static void CheckProcessorScheduling()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\PriorityControl");

            if (key == null || 24 != (int)key.GetValue("Win32PrioritySeparation", 2))
            {
                Log.WarnFormat("Processor scheduling is set to 'Programs', consider setting this to 'Background services'!");
                Thread.Sleep(3000);
            }
        }
    }
}
