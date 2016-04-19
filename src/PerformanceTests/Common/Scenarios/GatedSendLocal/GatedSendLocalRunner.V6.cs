#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class GatedSendLocalRunner
{
    async Task SendLocal(Command msg)
    {
        await this.EndpointInstance.SendLocal(msg);
    }

    public class Handler : IHandleMessages<Command>
    {
        public async Task Handle(Command message, IMessageHandlerContext ctx)
        {
            X.Signal();
        }
    }
}

#endif