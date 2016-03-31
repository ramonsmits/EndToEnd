namespace Common.Tests.TestCases
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using Types;

    public class TestInvalidCharacter : TestCase
    {
        public override Type MessageType => typeof(MessageWithInvalidCharacter);

        public override bool SupportsVersion(string version)
        {
            return !version.StartsWith("3.3");
        }

        public override void Populate(object instance)
        {
            var expected = (MessageWithInvalidCharacter)instance;

            var sb = new StringBuilder();
            sb.Append("Hello");
            sb.Append((char)0x1C);
            sb.Append("John");

            expected.Special = sb.ToString();
        }

        public override void CheckIfAreEqual(object instanceA, object instanceB)
        {
            var expected = (MessageWithInvalidCharacter)instanceA;
            var result = (MessageWithInvalidCharacter)instanceB;

            Assert.AreEqual(expected.Special, result.Special);
        }
    }
}