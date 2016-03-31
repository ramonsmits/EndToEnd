namespace Common.Tests.TestCases
{
    using System;
    using NUnit.Framework;
    using Types;

    public class TestPoco : TestCase
    {
        public override Type MessageType => typeof(Person);

        public override void Populate(object instance)
        {
            var expected = (Person)instance;

            expected.FirstName = "John";
            expected.LastName = "Smith";
        }

        public override void CheckIfAreEqual(object instanceA, object instanceB)
        {
            var expected = (Person)instanceA;
            var other = (Person)instanceB;

            Assert.AreEqual(expected.FirstName, other.FirstName);
            Assert.AreEqual(expected.LastName, other.LastName);
        }
    }
}