#if Version5
using NServiceBus;

partial class GatedSendLocalRunner : IProfile
{
    public IBus Bus { get; set; }

    void SendLocal(Command msg)
    {
        Bus.SendLocal(msg);
    }

    public class Handler : IHandleMessages<Command>
    {
        public void Handle(Command message)
        {
            X.Signal();
        }
    }

    public void Configure(BusConfiguration cfg)
    {
        cfg.PurgeOnStartup(true);
    }
}

#endif