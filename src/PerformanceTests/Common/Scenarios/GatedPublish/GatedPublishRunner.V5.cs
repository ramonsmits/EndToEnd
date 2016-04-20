#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedPublishRunner
{
    async Task Publish(object message)
    {
        EndpointInstance.Publish(message);
    }

    class Handler : IHandleMessages<Event>
    {
        public void Handle(Event message)
        {
            Signal();
        }
    }
}
#endif
