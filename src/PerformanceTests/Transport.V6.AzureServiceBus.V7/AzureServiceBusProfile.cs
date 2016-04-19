﻿using NServiceBus;
using NServiceBus.AzureServiceBus;

class AzureServiceBusProfile : IProfile
{
    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .UseTopology<ForwardingTopology>()
            .ConnectionString(this.GetConnectionString("AzureServiceBus"));
    }
}
