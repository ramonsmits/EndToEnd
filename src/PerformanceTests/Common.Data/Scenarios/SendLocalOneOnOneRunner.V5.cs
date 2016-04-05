#if Version5
using NServiceBus;

partial class SendLocalOneOnOneRunner
{
    public IBus Bus { get; set; }

    void SendLocal(Command msg)
    {
        Bus.SendLocal(msg);
    }

    public partial class Handler : IHandleMessages<Command>
    {
        public IBus Bus { get; set; }

        public void Handle(Command message)
        {
            if(Shutdown) return;
            Bus.SendLocal(message);
            System.Threading.Interlocked.Increment(ref Count);
        }
    }
}

#endif