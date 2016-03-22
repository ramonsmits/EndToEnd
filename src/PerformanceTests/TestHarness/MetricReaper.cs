using System;
using System.IO;
using Metrics;

public static class MetricReaper
{
    public static Meter Meter;

    public static void ConfigureMetrics(ExactOnce exactOnce, Persistence persistence)
    {
        var meterName = $"OutboxPerformanceTest using {exactOnce} and {persistence}";

        Meter = Metric.Meter(meterName, Unit.Commands, TimeUnit.Seconds);

        var folder = Path.Combine(Path.GetTempPath(), "reports");
        Console.WriteLine("Report output folder: {0}", folder);

        Metric.Config.WithReporting(report => report.WithCSVReports(folder, TimeSpan.FromSeconds(30)));
    }
}