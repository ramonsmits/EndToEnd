using System;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace PersistenceCompatibilityTests
{
    [TestFixture]
    public class NHibernatePersistenceTests : TestRun
    {
        [TestCaseSource(nameof(GenerateTestCases))]
        public void can_fetch_saga_persisted_by_another_version(string sourceVersion, string destinationVersion)
        {
            var sourceRunner = CreateTestFacade<ITestPersistence>(sourceVersion);
            var destinationRunner = CreateTestFacade<ITestPersistence>(destinationVersion);

            var id = Guid.NewGuid();
            var originator = sourceVersion;

            sourceRunner.Run(t => t.Persist(id, originator));
            destinationRunner.Run(t => t.Verify(id, originator));
        }

        static object[] GenerateTestCases()
        {
            var versions = new [] {"4.5", "6.2", "7.0"};

            var cases = from va in versions
                from vb in versions
                select new object[] {va, vb};

            return cases.ToArray();
        }
    }
}