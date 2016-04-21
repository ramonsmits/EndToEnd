#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class GatedPublishRunner
{
    class Handler : IHandleMessages<Event>
    {
        public Task Handle(Event message, IMessageHandlerContext context)
        {
            Signal();
            return Task.FromResult(0);
        }
    }
}

#endif
