namespace ServiceControlCompatibilityTests.Infrastructure
{
    using Autofac;
    using NServiceBus;

    class EndpointProxy
    {
        IEndpointInstance endpoint;

        public EndpointProxy(IStartableEndpoint endpoint, IContainer container)
        {
            this.endpoint = endpoint.Start().Result;
        }

        public void Send(string destination, object message)
        {
            endpoint.Send(destination, message).Wait();
        }
    }
}
