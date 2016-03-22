using NServiceBus;

namespace Messages.Notifications
{
    public class SensAnnouncementReceived : IEvent
    {
        public string StockCode { get; set; }
        public string Body { get; set; }
        public string Header { get; set; }
    }
}