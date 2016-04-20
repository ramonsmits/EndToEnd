#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedPublishRunner
{
    Task Publish(object message)
    {
        EndpointInstance.Publish(message);
        return Task.FromResult(0);
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
