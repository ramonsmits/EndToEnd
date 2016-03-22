using NServiceBus;
using System;

namespace Messages.Outgoing
{
    public class QueryTrade : IMessage
    {
        public Guid TradeId { get; set; }
    }
}