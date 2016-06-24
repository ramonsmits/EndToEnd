namespace ServiceControlCompatibilityTests
{
    using System.Threading.Tasks;
    using Infrastructure;
    using NServiceBus;
    using NServiceBus.Features;

    interface IEndpointFactory
    {
        Task<EndpointProxy> CreateEndpoint(string endpointName, IEndpointDetails endpointDetails = null);
    }

    class EndpointFactory : IEndpointFactory
    {
        ITransportDetails transportDetails;

        public EndpointFactory(ITransportDetails transportDetails)
        {
            this.transportDetails = transportDetails;
        }

        public async Task<EndpointProxy> CreateEndpoint(string endpointName, IEndpointDetails endpointDetails = null)
        {
            var config = new EndpointConfiguration(endpointName);
            config.UsePersistence<InMemoryPersistence>();
            config.EnableInstallers();
            config.PurgeOnStartup(true);
            config.DisableFeature<SecondLevelRetries>();
            config.DisableFeature<FirstLevelRetries>();

            if (endpointDetails != null)
            {
                var container = endpointDetails.CreateContainer();
                config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));
            }

            config.SendFailedMessagesTo("error");
            config.AuditProcessedMessagesTo("audit");

            transportDetails.ConfigureEndpoint(config);

            return new EndpointProxy(endpointName, await Endpoint.Create(config));
        }
    }
}