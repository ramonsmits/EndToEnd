#if Version6
using Cfg = NServiceBus.EndpointConfiguration;
using NServiceBus;
#else
using Cfg = NServiceBus.BusConfiguration;
#endif

using System;

class TimeToBeReceivedProfile : IProfile
{
    static readonly TimeSpan TTBR = TimeSpan.FromMinutes(1);
    public void Configure(Cfg cfg)
    {
        cfg.Conventions().DefiningTimeToBeReceivedAs(type => TTBR);
    }
}
