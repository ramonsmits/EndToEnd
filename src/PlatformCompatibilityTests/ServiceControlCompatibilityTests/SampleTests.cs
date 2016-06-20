namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;
    using System;

    [TestFixture]
    class SampleTests : SqlScTest
    {
        [TestCaseSource(nameof(AllTransports))]
        public void TransportTest(Type transportDetailType)
        {
            StartUp(transportDetailType);
            RunTest();
        }

        void RunTest()
        {
            Assert.IsTrue(ServiceControl.CheckIsAvailable(), "ServiceControl should be available");
        }
    }
}
