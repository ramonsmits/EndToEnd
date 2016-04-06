namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;
    using Variables;

    [TestFixture(Description = "Outbox vs DTC", Category = "Performance"), Explicit]
    public class OutboxVsDtcFixture : Base
    {
        [TestCaseSource(typeof(OutboxVsDtc), nameof(TestEnvironment.Generate))]
        public override void SendLocal(Permutation permutation)
        {
            base.SendLocal(permutation);
        }

        class OutboxVsDtc : TestEnvironment
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
                    Persisters = new[] { Persistence.NHibernate, Persistence.RavenDB, },
                    Serializers = new[] { Serialization.Json, },
                    MessageSizes = new[] { MessageSize.Tiny, MessageSize.Small, MessageSize.Medium, MessageSize.Large, },
                    OutboxModes = new[] { Outbox.Off, Outbox.On, },
                    DTCModes = new[] { DTC.Off, DTC.On, },
                    TransactionMode = new[] { TransactionMode.Default, },
                    AuditModes = new[] { Audit.Off },
                    ConcurrencyLevels = new[] { ConcurrencyLevel.EnvCores }
                });

            }
        }
    }
}