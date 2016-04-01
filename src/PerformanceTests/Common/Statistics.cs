using Metrics;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

[Serializable]
public class Statistics
{
    private static CountdownEvent countdownEvent;

    public DateTime? First;
    public DateTime Last;
    public DateTime StartTime;
    public long NumberOfMessages;
    public long NumberOfRetries;
    public TimeSpan SendTimeNoTx = TimeSpan.Zero;
    public TimeSpan SendTimeWithTx = TimeSpan.Zero;

    [NonSerialized]
    public Meter Meter;

    private static Statistics instance;
    public static Statistics Instance
    {
        get
        {
            if (instance == null) throw new InvalidOperationException("Statistics wasn't initialized. Call 'Initialize' first.");

            return instance;
        }
    }

    public static void Initialize(int numberOfMessages)
    {
        instance = new Statistics(numberOfMessages);
        instance.StartTime = DateTime.Now;
    }

    private Statistics(int numberOfMessages)
    {
        countdownEvent = new CountdownEvent(numberOfMessages);

        ConfigureMetrics();
    }

    public void Dump()
    {
        Console.Out.WriteLine("");
        Console.Out.WriteLine("---------------- Statistics ----------------");

        var durationSeconds = (Last - First.Value).TotalSeconds;

        PrintStats("NumberOfMessages", NumberOfMessages, "#");

        var throughput = Convert.ToDouble(NumberOfMessages) / durationSeconds;

        PrintStats("Throughput", throughput, "msg/s");

        Console.Out.WriteLine("##teamcity[buildStatisticValue key='ReceiveThroughput' value='{0}']", Math.Round(throughput));

        PrintStats("NumberOfRetries", NumberOfRetries, "#");
        PrintStats("TimeToFirstMessage", (First - StartTime).Value.TotalSeconds, "s");

        if (SendTimeNoTx != TimeSpan.Zero)
            PrintStats("Sending", Convert.ToDouble(NumberOfMessages / 2) / SendTimeNoTx.TotalSeconds, "msg/s");

        if (SendTimeWithTx != TimeSpan.Zero)
            PrintStats("SendingInsideTX", Convert.ToDouble(NumberOfMessages / 2) / SendTimeWithTx.TotalSeconds, "msg/s");
    }

    static void PrintStats(string key, double value, string unit)
    {
        Console.Out.WriteLine($"{key}: {value:0.0} ({unit})");
    }

    public void Signal()
    {
        Meter.Mark();
        countdownEvent.Signal();
    }

    public static void WaitUntilCompleted()
    {
        countdownEvent.Wait();
    }

    void ConfigureMetrics()
    {
        Meter = Metric.Meter("", Unit.Commands, TimeUnit.Seconds);

        var folder = Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "reports");

        Metric
            .Config.WithAllCounters()
            .WithReporting(report => report.WithCSVReports(folder, TimeSpan.FromSeconds(1)));
    }
}