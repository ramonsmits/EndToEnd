
using System.Threading.Tasks;
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

    public Task SendMessage(ISession session)
    {
        return session.SendLocal(new Command { Data = Data });
    }
}

