namespace ServiceControlCompatibilityTests.Infrastructure
{
    using System;
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

        public async Task<string> Send(string destination, object message)
        {
            var sendOptions = new SendOptions();

            var messageId = Guid.NewGuid().ToString();
            sendOptions.SetDestination(destination);
            sendOptions.SetMessageId(messageId);
            sendOptions.RouteReplyTo("Sender");

            await endpoint.Send(message, sendOptions).ConfigureAwait(false);

            return messageId;
        }

        public override string ToString()
        {
            return name;
        }

        public static implicit operator string(EndpointProxy p)
        {
            return p.name;
        }
    }
}
