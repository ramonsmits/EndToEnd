#if Version5
using NServiceBus;

partial class GatedSendLocalRunner
{
    class Handler : IHandleMessages<Command>
    {
        public void Handle(Command message)
        {
            Signal();
        }
    }
}
#endif
