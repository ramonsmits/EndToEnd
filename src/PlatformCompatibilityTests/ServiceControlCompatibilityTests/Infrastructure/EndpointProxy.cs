namespace ServiceControlCompatibilityTests.Infrastructure
{
    using System.Threading.Tasks;
    using NServiceBus;

    class EndpointProxy
    {
        IEndpointInstance endpoint;
        string name;

        public EndpointProxy(string name, IStartableEndpoint endpoint)
        {
            this.endpoint = endpoint.Start().Result;
            this.name = name;
        }

        public Task Send(string destination, object message, string messageId = null)
        {
            var sendOptions = new SendOptions();
            sendOptions.SetDestination(destination);
            if (messageId != null)
            {
                sendOptions.SetMessageId(messageId);
            }
            return endpoint.Send(message, sendOptions);
        }

        public static implicit operator string(EndpointProxy p)
        {
            return p.name;
        }
    }
}
