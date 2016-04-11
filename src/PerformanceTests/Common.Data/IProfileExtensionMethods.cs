namespace Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Text;

    public static class ProfileExtensionMethods
    {
        public static string GetConnectionString(this IProfile currentProfile, string connectionStringName)
        {
            var environmentVariableConnectionString = Environment.GetEnvironmentVariable(connectionStringName);
            if (!string.IsNullOrWhiteSpace((environmentVariableConnectionString)))
            {
                return environmentVariableConnectionString;
            }

            var applicationConfigConnectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            if (!string.IsNullOrWhiteSpace((applicationConfigConnectionString)))
            {
                return applicationConfigConnectionString;
            }

            throw new ConfigurationErrorsException(string.Format("Could not find an environment variable or connection string named {0}", connectionStringName));
        }
    }
}