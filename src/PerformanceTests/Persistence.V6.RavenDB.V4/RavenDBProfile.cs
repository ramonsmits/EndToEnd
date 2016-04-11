using NServiceBus;
using NServiceBus.Persistence;

class RavenDBProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.UsePersistence<RavenDBPersistence>();
    }
}
