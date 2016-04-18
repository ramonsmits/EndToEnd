#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedPublishRunner
{
    class Handler : IHandleMessages<Event>
    {
        public void Handle(Event message)
        {
            Signal();
        }
    }
}
#endif
