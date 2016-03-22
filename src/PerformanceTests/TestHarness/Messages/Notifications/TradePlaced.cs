using NServiceBus;

namespace Messages.Notifications
{
    public class TradePlaced : IEvent
    {
        public string StockCode { get; set; }
        public string Type { get; set; }
        public int Volume { get; set; }
    }
}