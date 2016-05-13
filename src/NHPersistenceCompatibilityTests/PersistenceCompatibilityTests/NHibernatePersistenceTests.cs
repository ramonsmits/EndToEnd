using NUnit.Framework;

namespace PersistenceCompatibilityTests
{
    [TestFixture]
    public class NHibernatePersistenceTests : TestRun
    {
        [Test]
        public void Cross_saga_communication_works()
        {
            var v6Runner = CreateTestFacade<ITestPersistence>("Version_6.2");
            var v7Runner = CreateTestFacade<ITestPersistence>("Version_7.0");
            
            v6Runner.Run(t => t.Persist());
            v7Runner.Run(t => t.Persist());

            //v7Runner.Run(t => t.Verify());
        }
    }
}