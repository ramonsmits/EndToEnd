
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
    protected override void Start()
    {
    }

    protected override void Stop()
    {
    }

    protected override Task Loop(object o)
    {
        while (!Shutdown)
        {
            try
            {

            }
    }

    public class Command : ICommand
    {
    }
}

