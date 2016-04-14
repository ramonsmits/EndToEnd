namespace TransportCompatibilityTests.Common
{
    using System;
    using System.Data.Common;
    using System.Net;

    public class RabbitConnectionStringBuilder
    {
        public static string EnvironmentVariable => "RabbitMQ.ConnectionString";
        public static string VirtualHostApiEnvironmentVariable => "RabbitMQ.VirtualHostAPI";
        public static string PermissionApiEnvironmentVariable => "RabbitMQ.PermissionAPI";

        public static string Build()
        {
            var value = Environment.GetEnvironmentVariable(EnvironmentVariable, EnvironmentVariableTarget.User);
            return value ?? Environment.GetEnvironmentVariable(EnvironmentVariable);
        }

        public static NetworkCredential Credentials()
        {
            var builder = new Builder(Build());

            return new NetworkCredential(builder["username"].ToString(), builder["password"].ToString());
        }

        public static Uri VirtualHostAPI()
        {
            var value = Environment.GetEnvironmentVariable(VirtualHostApiEnvironmentVariable, EnvironmentVariableTarget.User);
            return new Uri(value ?? Environment.GetEnvironmentVariable(VirtualHostApiEnvironmentVariable));
        }

        public static Uri PermissionsAPI()
        {
            var value = Environment.GetEnvironmentVariable(PermissionApiEnvironmentVariable, EnvironmentVariableTarget.User);
            return new Uri(value ?? Environment.GetEnvironmentVariable(PermissionApiEnvironmentVariable));
        }

        class Builder : DbConnectionStringBuilder
        {
            public Builder(string connectionString)
            {
                ConnectionString = connectionString;
            }
        }
    }
}