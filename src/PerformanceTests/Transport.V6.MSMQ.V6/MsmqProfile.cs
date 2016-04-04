
using NServiceBus;

class MsmqProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration.UseTransport<MsmqTransport>();
    }
}
