using System;
using NUnit.Framework;

namespace PersistenceCompatibilityTests
{
    [TestFixture]
    public class NHibernatePersistenceTests : TestRun
    {
        [Test]
        public void can_fetch_saga_persisted_by_another_version()
        {
            var v6Runner = CreateTestFacade<ITestPersistence>("NServiceBus.NHibernate.Tests_6.2");
            var v7Runner = CreateTestFacade<ITestPersistence>("NServiceBus.NHibernate.Tests_7.0");

            var id = Guid.NewGuid();

            v6Runner.Run(t => t.Persist(id, "v6.2"));
            v7Runner.Run(t => t.Verify(id, "v6.2"));
        }


        [Test]
        public void can_fetch_saga_persisted_by_another_version_2()
        {
            var v6Runner = CreateTestFacade<ITestPersistence>("NServiceBus.NHibernate.Tests_6.2");
            var v7Runner = CreateTestFacade<ITestPersistence>("NServiceBus.NHibernate.Tests_7.0");

            var id = Guid.NewGuid();

            v7Runner.Run(t => t.Persist(id, "v7.0"));
            v6Runner.Run(t => t.Verify(id, "v7.0"));
        }
    }
}