using System;
using System.Collections.Generic;
#if NHVersion7
using NServiceBus;
#else
using NServiceBus.Saga;
#endif

namespace Shared
{
    class TestSagaDataWithList : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual IList<int> Ints { get; set; }
    }
}
