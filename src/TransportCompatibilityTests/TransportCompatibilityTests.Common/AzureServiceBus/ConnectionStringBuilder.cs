namespace TransportCompatibilityTests.Common.AzureServiceBus
{
    using System;

    public class AzureServiceBusConnectionStringBuilder
    {
        public static string EnvironmentVariable => "AzureServiceBus.ConnectionString";

        public static string Build()
        {
            var value = Environment.GetEnvironmentVariable(EnvironmentVariable, EnvironmentVariableTarget.User);
            return value ?? Environment.GetEnvironmentVariable(EnvironmentVariable);
        }
    }
}