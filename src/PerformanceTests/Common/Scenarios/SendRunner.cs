
using System.Threading.Tasks;
#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
using NServiceBus;

/// <summary>
/// Does a continuous test where a pre-seeded amount of messages will be handled
/// </summary>    
class SendRunner : LoopRunner
{
    protected override int BatchSize { get; set; } = 16;

    protected override Task SendMessage()
    {
        throw new System.NotImplementedException();
    }

    protected override void Start()
    {
    }

    protected override void Stop()
    {
    }

    public class Command : ICommand
    {
    }
}

