using System;
using System.Configuration;
using NLog;

public static class ConfigurationHelper
{
    static readonly ILogger Log = LogManager.GetLogger(nameof(Configuration));

    public static string GetConnectionString(string connectionStringName)
    {
        var environmentVariableConnectionString = Environment.GetEnvironmentVariable(connectionStringName);
        if (!string.IsNullOrWhiteSpace(environmentVariableConnectionString))
        {
            Log.Info("Environment variable found {0}", environmentVariableConnectionString);
            return environmentVariableConnectionString;
        }

        var applicationConfigConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
        if (applicationConfigConnectionString != null)
        {
            Log.Info("App.config connection string variable found {0}", environmentVariableConnectionString);
            return applicationConfigConnectionString.ConnectionString;
        }

        return string.Empty;
    }

    public static string FetchSetting(string key)
    {
        string value;

        value = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrWhiteSpace(value))
        {
            Log.Info("Setting: {0} = {1} ({2})", key, value, "Environment");
            return value;
        }

        value = ConfigurationManager.AppSettings[key];

        if (!string.IsNullOrWhiteSpace(value))
        {
            Log.Info("Setting: {0} = {1} ({2})", key, value, "AppSetting");
            return value;
        }

        return null;
    }
}
