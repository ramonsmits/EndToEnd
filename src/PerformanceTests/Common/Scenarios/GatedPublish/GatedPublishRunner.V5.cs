#if Version5
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
