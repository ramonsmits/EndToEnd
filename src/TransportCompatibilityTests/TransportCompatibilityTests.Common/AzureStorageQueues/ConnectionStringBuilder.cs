namespace TransportCompatibilityTests.Common.AzureStorageQueues
{
    using System;

    public class AzureStorageQueuesConnectionStringBuilder
    {
        public static string EnvironmentVariable => "AzureStorageQueueTransport.ConnectionString";

        public static string Build()
        {
            var value = Environment.GetEnvironmentVariable(EnvironmentVariable, EnvironmentVariableTarget.User);
            return value ?? Environment.GetEnvironmentVariable(EnvironmentVariable);
        }
    }
}