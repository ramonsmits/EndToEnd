#if Version5
using NServiceBus;

partial class SendRunner
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