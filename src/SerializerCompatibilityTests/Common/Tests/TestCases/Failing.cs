namespace Common.Tests.TestCases
{
    using System;
    using NUnit.Framework;
    using Types;

    public class Failing : TestCase
    {
        public override Type MessageType => typeof(Person);


        //HINT: we need to put it and Passing here as Test.dll is not loaded into each custom nsb appdomains
        //      only Common.dll is. It should never run with other serialization test cases.
        public override bool SupportsVersion(string version)
        {
            return false;
        }

        public override void Populate(object instance)
        {
        }

        public override void CheckIfAreEqual(object instanceA, object instanceB)
        {
            Assert.AreEqual("0", "1");
        }
    }
}