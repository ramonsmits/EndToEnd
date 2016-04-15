#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedPublishRunner
{
    async Task Publish(object msg)
    {
        EndpointInstance.Publish(msg);
    }

    public class Handler : IHandleMessages<Event>
    {
        public void Handle(Event message)
        {
            X.Signal();
        }
    }
}
#endif
