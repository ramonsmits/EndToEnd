using System;
using System.Collections.Generic;
using System.Linq;
using NDesk.Options;

class Program
{
    static void Main()
    {
        var runAutomaticBenchmarks = ExecutionType.CommandParameter;
        var numberOfRuns = 3;
        var batches = 5;
        var exactOnce = ExactOnce.DTC;
        var persistence = Persistence.MSSQL;

        var parameterOptions = new OptionSet();
        parameterOptions.Add<int>("batches=", v =>
        {
            runAutomaticBenchmarks = runAutomaticBenchmarks == ExecutionType.CommandParameter ? ExecutionType.CommandParameter : ExecutionType.AutomaticBenchmark;
            batches = v;
        });
        parameterOptions.Add<int>("runs=", v =>
        {
            runAutomaticBenchmarks = runAutomaticBenchmarks == ExecutionType.CommandParameter ? ExecutionType.CommandParameter : ExecutionType.AutomaticBenchmark;
            numberOfRuns = v;
        });
        parameterOptions.Add<Persistence>("persistence=", v =>
        {
            runAutomaticBenchmarks = ExecutionType.CommandParameter;
            persistence = v;
        });
        parameterOptions.Add<ExactOnce>("exactOnce=", v =>
        {
            runAutomaticBenchmarks = ExecutionType.CommandParameter;
            exactOnce = v;
        });

        if (runAutomaticBenchmarks == ExecutionType.AutomaticBenchmark)
        {
            ExecuteAutomaticBenchmark(numberOfRuns, batches);
        }
        else if (runAutomaticBenchmarks == ExecutionType.Simple)
        {
            ExecuteSimpleScenario();
        }
        else
        {
            ExecuteConsoleParameters(numberOfRuns, exactOnce, persistence, batches);
        }
        Console.WriteLine("Press ENTER to quit...");
        Console.ReadLine();
    }

    private static void ExecuteConsoleParameters(
        int numberOfRuns,
        ExactOnce exactOnce,
        Persistence persistence,
        int batches
        )
    {
        var times = new List<TimeSpan>();

        for (var x = 0; x < numberOfRuns; x++)
        {
            using (var container = new HarnessRunContainer(exactOnce, persistence, batchesToSend: batches))
            {
                times.Add(container.Run());
                Console.WriteLine("{0}/{1} done in {2:g}", exactOnce, persistence, times.Last());
            }
        }

        Console.WriteLine("\n\n\nFinished running {0:N0} batches", numberOfRuns);
        Console.WriteLine("=========================================================");
        Console.WriteLine("Batch sizes (of 805 messages each): {0:N0}", batches);
        Console.WriteLine("Total messages each batch: {0:N0}", batches*805);
        Console.WriteLine("Total messages each: {0:N0}", batches*805*numberOfRuns);
        Console.WriteLine("=========================================================\n");

        Console.WriteLine("\tTotal time taken for {0}/{1}: {2:g}", exactOnce, persistence, times.Aggregate(TimeSpan.Zero, (acc, current) => acc + current));
        Console.WriteLine("\tMean time for {0}/{1}: {2:g}", exactOnce, persistence, TimeSpan.FromMilliseconds(times.Sum(time => time.TotalMilliseconds)/numberOfRuns));
        Console.WriteLine("\tStandard deviation of {0}/{1}: {2:N0}ms\n", exactOnce, persistence, times.Select(time => time.TotalMilliseconds).StandardDeviation());
    }

    private static void ExecuteAutomaticBenchmark(int numberOfRuns, int batches)
    {
        var measurements = new Dictionary<Tuple<ExactOnce, Persistence>, List<TimeSpan>>();

        foreach (ExactOnce exactOnce in Enum.GetValues(typeof(ExactOnce)))
            foreach (Persistence persistance in Enum.GetValues(typeof(Persistence)))
                for (var x = 0; x < numberOfRuns; x++)
                {
                    var configuration = Tuple.Create(exactOnce, persistance);
                    Console.WriteLine("\n== {0} ==\n", configuration);
                    if (!measurements.ContainsKey(configuration))
                    {
                        measurements[configuration] = new List<TimeSpan>();
                    }
                    using (var container = new HarnessRunContainer(exactOnce, persistance, batchesToSend: batches))
                    {
                        MetricReaper.ConfigureMetrics(exactOnce, persistance);

                        var duration = container.Run();
                        measurements[configuration].Add(duration);
                        Console.WriteLine("{0}/{1} done in {2:g}", exactOnce, persistance, duration);
                    }
                }


        Console.WriteLine("\n\n\nFinished running {0:N0} batches", numberOfRuns);
        Console.WriteLine("=========================================================");
        Console.WriteLine("Batch sizes (of 805 messages each): {0:N0}", batches);
        Console.WriteLine("Total messages each batch: {0:N0}", batches*805);
        Console.WriteLine("Total messages each: {0:N0}", batches*805*numberOfRuns);
        //Console.WriteLine("Seed size: {0:N0}", HarnessRunContainer.OutboxRecordCount);
        Console.WriteLine("=========================================================\n");

        var fastestInSeconds = measurements
            .Select(x => x.Value.Sum(y => y.TotalSeconds))
            .OrderBy(z => z)
            .First();

        foreach (var m in measurements)
        {
            Console.WriteLine("\n=={0}/{1}==\n", m.Key.Item1, m.Key.Item2);
            Console.WriteLine("\tTotal time taken    : {0,10:N}s",
                m.Value.Aggregate(TimeSpan.Zero, (acc, current) => acc + current).TotalSeconds);
            Console.WriteLine("\tMean time for       : {0,10:N}s",
                m.Value.Sum(time => time.TotalSeconds)/numberOfRuns);
            Console.WriteLine("\tStandard deviation  : {0,10:N}s\n",
                m.Value.Select(time => time.TotalSeconds).StandardDeviation());
            Console.WriteLine("\tRelative speed      : {0,10:N}%\n",
                m.Value.Sum(x => x.TotalSeconds)*100/fastestInSeconds);
        }
    }

    private static void ExecuteSimpleScenario()
    {
        Console.WriteLine("Do you want to use Outbox? (y/n)");
        var exactOnce = Console.ReadKey().Key == ConsoleKey.Y ? ExactOnce.Outbox : ExactOnce.DTC;

        Console.WriteLine("Do you want to use RavenDB? (y/n)");
        var persistance = Console.ReadKey().Key == ConsoleKey.Y ? Persistence.RavenDB : Persistence.MSSQL;

        using (var container = new HarnessRunContainer(exactOnce, persistance, 1))
        {
            //MetricReaper.ConfigureMetrics(useOutbox, useRaven);

            var timeTaken = container.Run();
            Console.WriteLine("Processing all messages took {0:g}", timeTaken);
        }
    }
}
