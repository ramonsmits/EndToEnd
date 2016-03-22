using NServiceBus;
using System;

namespace Messages.TradeActions
{
    public class PlaceShortTrade : IMessage
    {
        public Guid Id { get; set; }
        public string StockCode { get; set; }
        public decimal AskingPrice { get; set; }
        public int Volume { get; set; }
    }
}