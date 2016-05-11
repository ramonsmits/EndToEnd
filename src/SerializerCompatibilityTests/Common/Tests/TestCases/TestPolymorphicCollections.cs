namespace Common.Tests.TestCases
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Types;

    public class TestPolymorphicCollections : TestCase
    {
        public override Type MessageType => typeof(PolymorphicCollection);

        public override bool SupportsFormat(SerializationFormat format)
        {
            return format == SerializationFormat.Json;
        }

        public override void Populate(object instance)
        {
            var expected = (PolymorphicCollection)instance;

            expected.Items = new List<BaseEntity>
            {
                new SpecializationA
                {
                    Name = "A"
                },
                new SpecializationB
                {
                    Name = "B"
                }
            };
        }

        public override void CheckIfAreEqual(object instanceA, object instanceB)
        {
            var expected = (PolymorphicCollection)instanceA;
            var pc = (PolymorphicCollection)instanceB;

            CollectionAssert.AreEqual(expected.Items, pc.Items);
        }
    }
}