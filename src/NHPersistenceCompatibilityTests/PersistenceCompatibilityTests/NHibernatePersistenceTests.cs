using System;
using System.Collections.Generic;
using System.Linq;
using DataDefinitions;
using NUnit.Framework;

namespace PersistenceCompatibilityTests
{
    [TestFixture]
    public class NHibernatePersistenceTests : TestRun
    {
        [TestCaseSource(nameof(GenerateTestCases))]
        public void can_fetch_simple_saga_persisted_by_another_version(string sourceVersion, string destinationVersion)
        {
            var sourcePersister = PersisterFacadeCache[sourceVersion];
            var destinationPersister = PersisterFacadeCache[destinationVersion];

            var writeData = new TestSagaData
            {
                Id = Guid.NewGuid(),
                Originator = "test-originator"
            };

            sourcePersister.Save(writeData, nameof(writeData.Id), writeData.Id.ToString());

            var readData = destinationPersister.Get<TestSagaData>(writeData.Id);

            Assert.AreEqual(writeData.Id, readData.Id);
            Assert.AreEqual(writeData.Originator, readData.Originator);
        }

        [TestCaseSource(nameof(GenerateTestCases))]
        public void can_fetch_saga_with_list_persisted_by_another_version(string sourceVersion, string destinationVersion)
        {
            var sourcePersister = PersisterFacadeCache[sourceVersion];
            var destinationPersister = PersisterFacadeCache[destinationVersion];

            var writeData = new TestSagaDataWithList 
            {
                Id = Guid.NewGuid(),
                Ints = new List<int> { 1, 5, 7, 9, -3}
            };

            sourcePersister.Save(writeData, nameof(writeData.Id), writeData.Id.ToString());

            var readData = destinationPersister.Get<TestSagaDataWithList>(writeData.Id);

            Assert.AreEqual(writeData.Id, readData.Id);
            CollectionAssert.AreEqual(writeData.Ints, readData.Ints);
        }

        [TestCaseSource(nameof(GenerateTestCases))]
        public void can_fetch_composite_saga_persisted_by_another_version(string sourceVersion, string destinationVersion)
        {
            var sourcePersister = PersisterFacadeCache[sourceVersion];
            var destinationPersister = PersisterFacadeCache[destinationVersion];

            var writeData = new TestSagaDataWithComposite
            {
                Id = Guid.NewGuid(),
                Composite = new TestSagaDataWithComposite.SagaComposite { Value = "test-value" }
            };

            sourcePersister.Save(writeData, nameof(writeData.Id), writeData.Id.ToString());

            var readData = destinationPersister.Get<TestSagaDataWithComposite>(writeData.Id);

            Assert.AreEqual(writeData.Id, readData.Id);
            CollectionAssert.AreEqual(writeData.Composite.Value, readData.Composite.Value);
        }

        public override void OneTimeSetup()
        {
            PersisterFacadeCache = new Dictionary<string, PersisterFacade>();

            var combinations = GenerateTestCases();
            foreach (var versionPair in combinations)
            {
                var vFrom = versionPair[0].ToString();
                var vTo = versionPair[1].ToString();

                if (!PersisterFacadeCache.ContainsKey(vFrom))
                {
                    PersisterFacadeCache.Add(vFrom, CreatePersister(vFrom));
                }

                if (!PersisterFacadeCache.ContainsKey(vTo))
                {
                    PersisterFacadeCache.Add(vTo, CreatePersister(vTo));
                }
            }
        }

        static object[][] GenerateTestCases()
        {
            var versions = new [] {"4.5", "5.0", "6.2", "7.0"};

            var cases = from va in versions
                from vb in versions
                select new object[] {va, vb};

            return cases.ToArray();
        }

        Dictionary<string, PersisterFacade> PersisterFacadeCache;
    }
}