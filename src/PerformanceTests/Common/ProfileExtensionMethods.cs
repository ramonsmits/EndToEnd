using System;
using System.Configuration;
using NServiceBus.Logging;

public static class ProfileExtensionMethods
{
    static readonly ILog Log = LogManager.GetLogger(typeof(ProfileExtensionMethods));

    public static string GetConnectionString(this IProfile currentProfile, string connectionStringName)
    {
        var environmentVariableConnectionString = Environment.GetEnvironmentVariable(connectionStringName);
        if (!string.IsNullOrWhiteSpace(environmentVariableConnectionString))
        {
            Log.InfoFormat("Environment variable found {0}", environmentVariableConnectionString);
            return environmentVariableConnectionString;
        }

        var applicationConfigConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
        if (applicationConfigConnectionString != null)
        {
            Log.InfoFormat("App.config connection string variable found {0}", environmentVariableConnectionString);
            return applicationConfigConnectionString.ConnectionString;
        }

        return string.Empty;
    }

    public static string FetchSetting(this IProfile instance, string key)
    {
        string value;

        value = Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrWhiteSpace(value))
        {
            Log.InfoFormat("Setting: {0} = {1} ({2})", key, value, "Environment");
            return value;
        }

        value = ConfigurationManager.AppSettings[key];

        if (!string.IsNullOrWhiteSpace(value))
        {
            Log.InfoFormat("Setting: {0} = {1} ({2})", key, value, "AppSetting");
            return value;
        }

        return null;
    }
}
