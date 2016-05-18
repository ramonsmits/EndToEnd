using System;
#if NHVersion7
using NServiceBus;
#else
using NServiceBus.Saga;
#endif

namespace Shared
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