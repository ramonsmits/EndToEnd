#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class SendRunner
{
    Task SendLocal(Command msg)
    {
        EndpointInstance.SendLocal(msg);
        return Task.FromResult(0);
    }

    class Handler : IHandleMessages<Command>
    {
        public void Handle(Command message)
        {
            Signal();
        }
    }
}
#endif