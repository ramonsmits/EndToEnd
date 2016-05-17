using System;
using System.Collections.Generic;
using NServiceBus.Saga;

namespace Version_6_2
{
    class TestSagaDataWithList : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual IList<int> Ints { get; set; }
    }
}