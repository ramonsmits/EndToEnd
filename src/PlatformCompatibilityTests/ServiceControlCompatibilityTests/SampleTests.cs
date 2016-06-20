namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;
    using System;
    using System.Linq;

    [TestFixture]
    class SampleTests : SqlScTest
    {
        [Test]
        public async void SampleTestA()
        {
            var details = await ServiceControl.Get<ServiceControlDetails>("");

            Assert.Pass($"{TestContext.CurrentContext.Test.Name} - {details.Description}");
        }

        [Test]
        public async void SampleTestB()
        {
            var details = await ServiceControl.Get<ServiceControlDetails>("");

            Assert.Pass($"{TestContext.CurrentContext.Test.Name} - {details.Description}");
        }

        [TestCaseSource(nameof(TransportTests))]
        public void TransportTest(Type transport)
        {
            RunTest(ActivateInstanceOfTransportDetail(transport));
        }

        public void RunTest(ITransportDetails transport)
        {
            StartServiceControl(transport);

            Assert.Pass();
        }

        static object[] TransportTests()
        {
            var transports = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Any(i => i == (typeof(ITransportDetails))));

            return transports.ToArray();
        }

        class ServiceControlDetails
        {
            public string Description { get; set; }
        }
    }
}
