namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Variables;

    [TestFixture]
    public class ResourceUtilizationFixture : Base
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
                Platforms = new[] { Platform.x86, Platform.x64, },
                GarbageCollectors = new[] { GarbageCollector.Client,GarbageCollector.Server,  },
                Transports = new[] { Transport.MSMQ },
                Persisters = new[] { Persistence.InMemory },
                Serializers = new[] { Serialization.Json, },
                MessageSizes = new[] { MessageSize.Tiny, },
                OutboxModes = new[] { Outbox.Off },
                DTCModes = new[] { DTC.On, },
                TransactionMode = new[] { TransactionMode.Default, },
                AuditModes = new[] { Audit.On, },
                ConcurrencyLevels = new[] { ConcurrencyLevel.EnvCores }
            });
        }
    }
}