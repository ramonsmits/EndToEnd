namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;
    using Variables;

    [TestFixture(Description = "Publish vs Send")]
    public class PublishVsSendFixture : Base
    {
        [TestCaseSource(typeof(PublishVsSend), nameof(TestEnvironment.Generate))]
        public override void PublishToSelf(Permutation permutation)
        {
            base.PublishToSelf(permutation);
        }

        [TestCaseSource(typeof(PublishVsSend), nameof(TestEnvironment.Generate))]
        public override void SendLocal(Permutation permutation)
        {
            base.SendLocal(permutation);
        }

        [TestCaseSource(typeof(PublishVsSend), nameof(TestEnvironment.Generate))]
        public override void SendToSelf(Permutation permutation)
        {
            base.SendToSelf(permutation);
        }

        class PublishVsSend : TestEnvironment
        {
            protected override IEnumerable<Permutation> CreatePermutations()
            {
                return PermutationGenerator.Generate(new Permutations
                {
                    Versions = new[] { NServiceBusVersion.V5, NServiceBusVersion.V6, },
                    IOPS = new[] { IOPS.Default },
                    Platforms = new[] { Platform.x86, },
                    GarbageCollectors = new[] { GarbageCollector.Client, },
                    Transports = new[] { Transport.MSMQ, Transport.ASB, Transport.ASQ, Transport.MSMQ, Transport.RMQ, Transport.SQL, },
                    Persisters = new[] { Persistence.InMemory },
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
}