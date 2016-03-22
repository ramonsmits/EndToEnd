using System;
using NServiceBus;

namespace Messages.Incoming
{
    public class SensAnnouncement : IMessage
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public string StockCode { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }
        public bool ShouldMessageFail { get; set; }
    }
}