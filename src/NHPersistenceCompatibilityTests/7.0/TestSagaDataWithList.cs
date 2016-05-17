using System;
using System.Collections.Generic;
using NServiceBus;

namespace Version_7_0
{
    public class TestSagaDataWithList : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual IList<int> Ints { get; set; }
    }
}
