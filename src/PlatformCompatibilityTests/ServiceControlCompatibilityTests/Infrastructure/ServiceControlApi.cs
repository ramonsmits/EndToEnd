namespace ServiceControlCompatibilityTests
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Threading;
    using System.Linq;

    public class ServiceControlApi
    {
        public ServiceControlApi(string rootUri)
        {
            this.rootUri = rootUri;
        }

        public bool CheckIsAvailable()
        {
            return Get<object>("").Result != null;
        }

        public async Task<FailedMessage> WaitForFailedMessage(string failedMessageId)
        {
            while (true)
            {
                var errors = await GetAllErrors().ConfigureAwait(false);
                var failedMessage = errors.SingleOrDefault(x => x.MessageId == failedMessageId);
                if (failedMessage != null)
                    return failedMessage;
                await Task.Delay(200);
            }
        }

        public async Task<FailedMessage> WaitForNewFailingMessages(string endpointName)
        {
            // NB: Not threadsafe! If some other process is interfering, this code is completely broken.
            // Perhaps we should use a message mutator to hardcode the message id we're looking for instead?
            var startMessages = await GetErrorsForEndpoint<IList<FailedMessage>>(endpointName).ConfigureAwait(false);
            var startCount = startMessages.Count;

            IList<FailedMessage> currentMessages;
            int newCount;

            do
            {
                Thread.Sleep(200);
                currentMessages = await GetErrorsForEndpoint<IList<FailedMessage>>(endpointName).ConfigureAwait(false);
                newCount = currentMessages.Count;
            } while (newCount <= startCount);

            var newFailedMessage = currentMessages.FirstOrDefault(m => !startMessages.Select(sm => sm.Id).Contains(m.Id));

            return newFailedMessage;
        }

        internal Task RetryMessageId(string messageid)
        {
            return Post<object>($"/errors/{messageid}/retry", new { MessageId = messageid });
        }

        public Task<T> GetErrorsForEndpoint<T>(string endpointName) where T : class
        {
            return Get<T>($"/endpoints/{endpointName}/errors");
        }

        public Task<IList<FailedMessage>> GetAllErrors()
        {
            return Get<IList<FailedMessage>>("/errors");
        }

        public Task<T> Post<T>(string url, object parameters) where T : class
        {
            return ExecuteRequest<T>(url, "POST", JsonConvert.SerializeObject(parameters));
        }

        public Task<T> Get<T>(string url) where T : class
        {
            return ExecuteRequest<T>(url, "GET");
        }

        // TODO: This was lifted from the SC Acceptance Tests. It may not be appropriate for these tests
        async Task<T> ExecuteRequest<T>(string url, string httpMethod, string requestBodyParameters = null) where T : class
        {
            var request = (HttpWebRequest)WebRequest.Create($"{rootUri}{url}");
            request.Method = httpMethod;
            request.Accept = "application/json";

            if (!string.IsNullOrWhiteSpace(requestBodyParameters))
            {
                var encoder = new System.Text.ASCIIEncoding();
                var data = encoder.GetBytes(requestBodyParameters);
                request.GetRequestStream().Write(data, 0, data.Length);
            }

            HttpWebResponse response;
            try
            {
                response = await request.GetResponseAsync() as HttpWebResponse;
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

            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Accepted)
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

        // Copied from ServiceControl (with omissions).
        // Is there a better way for us to do this?
        public class FailedMessage
        {
            public string Id { get; set; }
            public string MessageType { get; set; }
            public DateTime? TimeSent { get; set; }
            public bool IsSystemMessage { get; set; }
            public string MessageId { get; set; }
            public int NumberOfProcessingAttempts { get; set; }
            public DateTime TimeOfFailure { get; set; }
            public DateTime LastModified { get; set; }
        }
    }
}