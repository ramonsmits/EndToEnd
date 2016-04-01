namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Variables;

    [TestFixture(Description = "Persisters")]
    public class PersistersFixture : Base
    {
        [Test]
        [TestCaseSource(nameof(Generate))]
        public override void SendLocal(Permutation permutation)
        {
            base.SendLocal(permutation);
        }

        IEnumerable<Permutation> Generate()
        {
            return PermutationGenerator.Generate(new Permutations
            {
                Versions = new[] { NServiceBusVersion.V5, NServiceBusVersion.V6, },
                IOPS = new[] { IOPS.Normal },
                Platforms = new[] { Platform.x86, },
                GarbageCollectors = new[] { GarbageCollector.Client, },
                Transports = new[] { Transport.MSMQ },
                Persisters = new[] { Persistence.InMemory, Persistence.NHibernate, Persistence.RavenDB },
                Serializers = new[] { Serialization.Json, },
                MessageSizes = new[] { MessageSize.Tiny, },
                OutboxModes = new[] { Outbox.Off, Outbox.On, },
                DTCModes = new[] { DTC.Off, DTC.On, },
                TransactionMode = new[] { TransactionMode.Default, },
                AuditModes = new[] { Audit.Off },
                ConcurrencyLevels = new[] { ConcurrencyLevel.EnvCores }
            });
        }
    }
}