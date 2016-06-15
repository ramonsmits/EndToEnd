namespace ServiceControlCompatibilityTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class ServiceControlApi
    {
        public ServiceControlApi(string rootUri)
        {
            this.rootUri = rootUri;
        }

        public bool CheckIsAvailable()
        {
            return Get<object>("") != null;
        }

        // TODO: This was lifted from the SC Acceptance Tests. It may not be appropriate for these tests
        public T Get<T>(string url) where T : class
        {
            var request = (HttpWebRequest)WebRequest.Create($"{rootUri}{url}");
            request.Accept = "application/json";

            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
            }

            if (response == null)
            {
                return null;
            }

            //for now
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return null;
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"Call failed: {(int)response.StatusCode} - {response.StatusDescription}");
            }

            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                {
                    return null;
                }

                var serializer = JsonSerializer.Create(serializerSettings);

                return serializer.Deserialize<T>(new JsonTextReader(new StreamReader(stream)));
            }
        }


        string rootUri;

        static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new UnderscoreMappingResolver(),
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            Converters =
            {
                new IsoDateTimeConverter
                {
                    DateTimeStyles = DateTimeStyles.RoundtripKind
                },
                new StringEnumConverter
                {
                    CamelCaseText = true
                }
            }
        };
    }
}