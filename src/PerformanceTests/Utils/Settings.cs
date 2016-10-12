using System;
using System.Configuration;
using System.Globalization;

public static class Settings
{
    public static TimeSpan WarmupDuration = TimeSpan.Parse(ConfigurationManager.AppSettings["WarmupDuration"]);
    public static TimeSpan RunDuration = TimeSpan.Parse(ConfigurationManager.AppSettings["RunDuration"]);
    public static double SeedDurationFactor = Convert.ToDouble(ConfigurationManager.AppSettings["SeedDurationFactor"], CultureInfo.InvariantCulture);
    public static TimeSpan SeedDuration => TimeSpan.FromSeconds((RunDuration + WarmupDuration).TotalSeconds * SeedDurationFactor);
}
