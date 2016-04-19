#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedSendLocalRunner
{
    async Task SendLocal(Command msg)
    {
        this.EndpointInstance.SendLocal(msg);
    }

    public class Handler : IHandleMessages<Command>
    {
        public void Handle(Command message)
        {
            X.Signal();
        }
    }
}
#endif
