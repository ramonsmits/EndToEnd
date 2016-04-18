#if Version5
using System.Threading.Tasks;
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
