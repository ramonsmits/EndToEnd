using System;
#if NHVersion7
using NServiceBus;
#else
using NServiceBus.Saga;
#endif

namespace Shared
{
    public class TestSagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
    }
}