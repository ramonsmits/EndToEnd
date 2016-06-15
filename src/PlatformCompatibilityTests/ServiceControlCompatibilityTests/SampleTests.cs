namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;

    [TestFixture]
    class SampleTests : SqlScTest
    {
        [Test]
        public void SampleTestA()
        {
            var details = ServiceControl.Get<ServiceControlDetails>("");

            Assert.Pass($"{TestContext.CurrentContext.Test.Name} - {details.Description}");
        }

        [Test]
        public void SampleTestB()
        {
            var details = ServiceControl.Get<ServiceControlDetails>("");

            Assert.Pass($"{TestContext.CurrentContext.Test.Name} - {details.Description}");
        }

        class ServiceControlDetails
        {
            public string Description { get; set; }
        }
    }
}
