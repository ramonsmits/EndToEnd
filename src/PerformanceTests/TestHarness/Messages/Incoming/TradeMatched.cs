using System;
using NServiceBus;

namespace Messages.Incoming
{
    public class TradeMatched : IMessage
    {
        public string StockCode { get; set; }
        public DateTime Time { get; set; }
        public decimal TradePrice { get; set; }
        public int TradeVolume { get; set; }
        public Guid Id { get; set; }
        public bool ShouldMessageFail { get; set; }
    }
}
