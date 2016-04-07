#if Version5
using NServiceBus;

partial class GatedSendLocalRunner : IProfile
{
    void SendLocal(Command msg)
    {
        NServiceBus5.Program.Instance.SendLocal(msg);
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
