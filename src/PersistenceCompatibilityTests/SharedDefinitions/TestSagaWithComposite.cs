﻿using System;

namespace DataDefinitions
{
    // ReSharper disable once PartialTypeWithSinglePart
    public partial class TestSagaDataWithComposite 
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
