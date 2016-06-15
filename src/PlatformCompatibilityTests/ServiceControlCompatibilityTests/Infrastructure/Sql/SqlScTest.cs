namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;

    abstract class SqlScTest
    {
        ServiceControlInstance serviceControl;

        [SetUp]
        public void Setup()
        {
            var context = TestContext.CurrentContext;

            // TODO: This is a build artifact from the latest SC build. It should be imported into the working folder of the build
            var factory = new ServiceControlFactory(@"C:\Temp\ServiceControl");

            // TODO: ServiceControl SQL Broker Connection String
            var transport = new SqlTransportDetails("Data Source=.\\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True");

            // TODO: Use transport to Clean Up

            serviceControl = factory.Start(transport, context.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            serviceControl.Stop();
        }

        protected ServiceControlApi ServiceControl => serviceControl.Api;
    }
}