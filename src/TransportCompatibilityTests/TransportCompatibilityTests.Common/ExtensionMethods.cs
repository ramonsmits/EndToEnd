namespace TransportCompatibilityTests.Common
{
    using System;
    using TransportCompatibilityTests.Common.AzureServiceBus;
    using TransportCompatibilityTests.Common.RabbitMQ;
    using TransportCompatibilityTests.Common.SqlServer;

    public static class ExtensionMethods
    {
        public static T As<T>(this object target)
        {
            return (T) target;
        }

        public static string TransportAddressForVersion(this RabbitMqEndpointDefinition endpointDefinition, int version)
        {
            return endpointDefinition.Name + "." + Environment.MachineName;
        }

        public static string TransportAddressForVersion(this SqlServerEndpointDefinition endpointDefinition, int version)
        {
            return (version == 1) ? endpointDefinition.Name + "." + Environment.MachineName : endpointDefinition.Name;
        }
        public static string TransportAddressForVersion(this AzureServiceBusEndpointDefinition endpointDefinition, int version)
        {
            return endpointDefinition.Name;
        }
    }
}
