using System.Threading.Tasks;
using NServiceBus;

/// <summary>
/// Does a continuous test where a pre-seeded amount of messages will be handled
/// </summary>    
partial class SendRunner : LoopRunner
{
    protected override Task SendMessage()
    {
        return Session.SendLocal(new Command { Data = Data });
    }

    public class Command : ICommand
    {
        public byte[] Data { get; set; }
    }

    public class Handler : Handler<Command>
    {
    }
}


