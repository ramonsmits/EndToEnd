#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class SendLocalOneOnOneRunner
{
    public IEndpointInstance Bus { get; set; }

    void SendLocal(Command msg)
    {
        Bus.SendLocal(msg);
    }

    public partial class Handler : IHandleMessages<Command>
    {
        public async Task Handle(Command message, IMessageHandlerContext ctx)
        {
            if (Shutdown) return;
            await ctx.SendLocal(message);
            System.Threading.Interlocked.Increment(ref Count);
        }
    }
}

#endif