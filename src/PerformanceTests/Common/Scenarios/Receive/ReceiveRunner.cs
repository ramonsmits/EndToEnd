#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
using NServiceBus;

/// <summary>
/// Does a continuous test where a pre-seeded amount of messages will be handled
/// </summary>    
partial class ReceiveRunner : BaseRunner
{
    public class Command : ICommand
    {
        public byte[] Data { get; set; }
    }

    partial class Handler
    {
    }
}

