
using NServiceBus;

class MsmqProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        transport.ConnectionString("deadLetter=false;journal=false");
    }
}
