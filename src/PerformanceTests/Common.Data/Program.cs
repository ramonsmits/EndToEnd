namespace Host
{
    using System;
    using System.Linq;
    using Common;
    using Tests.Permutations;
    using Utils;
    using VisualStudioDebugHelper;

    partial class Program
    {
        static string endpointName = "PerformanceTests_" + AppDomain.CurrentDomain.FriendlyName.Replace(' ', '_');
        static void Main(string[] args)
        {
            DebugAttacher.AttachDebuggerToVisualStudioProcessFromCommandLineParameter();

            try
            {
                TraceLogger.Initialize();

                Statistics.Initialize();

                Log.Env();

                var permutation = PermutationParser.FromCommandlineArgs();
                var options = BusCreationOptions.Parse(args);

                if (Environment.UserInteractive) Console.Title = permutation.ToString();

                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var tasks = permutation.Tests.Select(x => (IStartAndStop)assembly.CreateInstance(x)).ToArray();

                Run(options, permutation, tasks);
            }
            catch (Exception ex)
            {
                NServiceBus.Logging.LogManager.GetLogger(typeof(Program)).Fatal("Main", ex);
                throw;
            }
        }
    }
}
