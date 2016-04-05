namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;
    using Variables;

    [TestFixture(Description = "Audit forwarding On / Off")]
    public class AuditOnVsOffFixture : Base
    {
        [Test]
        [TestCaseSource(typeof(AuditOnVsOff), nameof(TestEnvironment.Generate))]
        public override void SendLocal(Permutation permutation)
        {
            base.SendLocal(permutation);
        }

        class AuditOnVsOff : TestEnvironment
        {
            protected override IEnumerable<Permutation> CreatePermutations()
            {
                return PermutationGenerator.Generate(new Permutations
                {
                    Versions = new[] { NServiceBusVersion.V5, NServiceBusVersion.V6, },
                    IOPS = new[] { IOPS.Default },
                    Platforms = new[] { Platform.x86, },
                    GarbageCollectors = new[] { GarbageCollector.Client, },
                    Transports = new[] { Transport.MSMQ },
                    Persisters = new[] { Persistence.InMemory },
                    Serializers = new[] { Serialization.Json, },
                    MessageSizes = new[] { MessageSize.Tiny, MessageSize.Small, MessageSize.Medium, MessageSize.Large, },
                    OutboxModes = new[] { Outbox.Off, Outbox.On, },
                    DTCModes = new[] { DTC.Off, DTC.On, },
                    TransactionMode = new[] { TransactionMode.Default, },
                    AuditModes = new[] { Audit.Off, Audit.On, },
                    ConcurrencyLevels = new[] { ConcurrencyLevel.EnvCores }
                });
            }
        }
    }
}