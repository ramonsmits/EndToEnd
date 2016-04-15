namespace TransportCompatibilityTests.RabbitMQ.Infrastructure
{
    using System;
    using System.IO;
    using System.Net;
    using NUnit.Framework;
    using TransportCompatibilityTests.Common;

    [TestFixture]
    public abstract class RabbitMqContext
    {
        [SetUp]
        public void CommonSetUp()
        {
            if (string.IsNullOrWhiteSpace(RabbitConnectionStringBuilder.Build()))
            {
                throw new Exception($"Environment variables `{RabbitConnectionStringBuilder.EnvironmentVariable}`, `{RabbitConnectionStringBuilder.PermissionApiEnvironmentVariable}` and `{RabbitConnectionStringBuilder.VirtualHostApiEnvironmentVariable}` are required to connect to RabbitMQ.");
            }

            DeleteVirtualHost();
            CreateVirtualHost();
            SetupPermission();
        }

        private static void DeleteVirtualHost()
        {
            var request = (HttpWebRequest)WebRequest.Create(RabbitConnectionStringBuilder.VirtualHostAPI());
            request.Credentials = RabbitConnectionStringBuilder.Credentials();
            request.Method = "DELETE";

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.NoContent && response.StatusCode != HttpStatusCode.NotFound)
                    {
                        // What's the best exception type here?
                        throw new Exception();
                    }
                }
            }
            catch (WebException e)
            {
                //TODO: this needs to be fixed. Not sure if we can make it better with WebRequest
                if (e.Message.Contains("The remote server returned an error: (404) Not Found.") == false)
                {
                    throw;
                }
            }
        }

        private static void CreateVirtualHost()
        {
            var request = (HttpWebRequest)WebRequest.Create(RabbitConnectionStringBuilder.VirtualHostAPI());
            request.Credentials = RabbitConnectionStringBuilder.Credentials();
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    // What's the best exception type here?
                    throw new Exception();
                }
            }
        }

        private static void SetupPermission()
        {
            var request = (HttpWebRequest)WebRequest.Create(RabbitConnectionStringBuilder.PermissionsAPI());
            request.Credentials = RabbitConnectionStringBuilder.Credentials();
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream))
            {
                writer.Write(@"{""configure"":"".*"",""write"":"".*"",""read"":"".*""}");
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    // What's the best exception type here?
                    throw new Exception();
                }
            }
        }
    }
}
