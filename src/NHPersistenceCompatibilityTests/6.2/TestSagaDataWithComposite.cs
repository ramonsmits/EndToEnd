using System;
using NServiceBus.Saga;

namespace Version_6_2
{
    public class TestSagaDataWithComposite : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual SagaComposite Composite { get; set; }

        public class SagaComposite
        {
            public virtual string Value { get; set; }
        }
    }
}