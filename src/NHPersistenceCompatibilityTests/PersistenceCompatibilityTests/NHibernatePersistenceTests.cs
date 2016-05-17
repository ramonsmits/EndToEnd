using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace PersistenceCompatibilityTests
{
    [TestFixture]
    public class NHibernatePersistenceTests : TestRun
    {
        [TestCase("6.2", "7.0")]
        [TestCase("7.0", "6.2")]
        public void can_fetch_saga_persisted_by_another_version(string sourceVersion, string destinationVersion)
        {
            var sourceRunner = CreateTestFacade<ITestPersistence>(sourceVersion);
            var destinationRunner = CreateTestFacade<ITestPersistence>(destinationVersion);

            var id = Guid.NewGuid();
            var originator = sourceVersion;

            sourceRunner.Run(t => t.Persist(id, originator));
            destinationRunner.Run(t => t.Verify(id, originator));
        }
    }
}