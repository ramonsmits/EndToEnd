using System.Threading.Tasks;
using NServiceBus;

abstract class SendLoop : BaseLoop
{

    protected SendLoop()
    {
        SendOnly = true;
    }
    protected override Task SendMessage()
    {
        return Session.SendLocal(new Command { Data = Data });
    }

    class Command : ICommand
    {
        public byte[] Data { get; set; }
    }
}