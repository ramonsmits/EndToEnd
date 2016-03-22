using NServiceBus;
using System;

namespace Messages.TradeActions
{
    public class PlaceLongTrade : IMessage
    {
        public Guid Id { get; set; }
        public string StockCode { get; set; }
        public decimal OfferPrice { get; set; }
        public int Volume { get; set; }
    }
}