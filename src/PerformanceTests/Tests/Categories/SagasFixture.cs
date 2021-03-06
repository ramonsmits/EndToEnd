namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Variables;

    [TestFixture(Description = "Sagas", Category = "Performance")]
    public class SagasFixture : Base
    {
        [TestCaseSource(nameof(CreatePermutations))]
        public void SagaUpdateRunner(Permutation permutation)
        {
            Tasks(permutation);
        }

        [TestCaseSource(nameof(CreatePermutations))]
        public override void SagaInitiateRunner(Permutation permutation)
        {
            base.SagaInitiateRunner(permutation);
        }

        [TestCaseSource(nameof(CreatePermutations))]
        public void SagaCongestionRunner(Permutation permutation)
        {
            Tasks(permutation);
        }

        static IEnumerable<Permutation> CreatePermutations()
        {
            return PermutationGenerator.Generate(new Permutations
            {
                Transports = new[] { Transport.MSMQ },
                Persisters = new[] { Persistence.Azure, Persistence.NHibernate, Persistence.RavenDB, },
                Serializers = new[] { Serialization.Json, },
                OutboxModes = new[] { Outbox.Off, },
                ConcurrencyLevels = new[] { ConcurrencyLevel.Sequential, ConcurrencyLevel.EnvCores04x, },
                TransactionMode = new[] { TransactionMode.Atomic, TransactionMode.Transactional, }
            },
            p => p.Persister == Persistence.Azure && p.TransactionMode != TransactionMode.Transactional || p.Persister != Persistence.Azure
            );
        }
    }
}
