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
partial class SendRunner : LoopRunner
{
    protected override async Task SendMessage()
    {
        await this.SendLocal(new Command());
    }

    public class Command : ICommand
    {
    }
}

