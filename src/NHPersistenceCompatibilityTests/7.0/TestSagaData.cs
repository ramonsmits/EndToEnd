using System;
using NServiceBus;
using NServiceBus.SagaPersisters.NHibernate;

namespace Version_7_0
{
    public class TestSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
    }
}