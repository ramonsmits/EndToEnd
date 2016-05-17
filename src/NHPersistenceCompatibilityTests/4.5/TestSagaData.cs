using System;
using NServiceBus.Saga;

namespace Version_4_5
{
    public class TestSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
    }
}