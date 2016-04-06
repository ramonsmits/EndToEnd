namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;
    using Variables;

    [TestFixture(Description = "Resources utilization", Category = "Performace"), Explicit]
    public class ResourceUtilizationFixture : Base
    {
        [TestCaseSource(typeof(ResourceUtilization), nameof(TestEnvironment.Generate))]
        public override void SendLocal(Permutation permutation)
        {
            base.SendLocal(permutation);
        }

        class ResourceUtilization : TestEnvironment
        {
            protected override IEnumerable<Permutation> CreatePermutations()
            {
                return PermutationGenerator.Generate(new Permutations
                {
                    Versions = new[] { NServiceBusVersion.V5, NServiceBusVersion.V6, },
                    IOPS = new[] { IOPS.Default },
                    Platforms = new[] { Platform.x86, Platform.x64, },
                    GarbageCollectors = new[] { GarbageCollector.Client, GarbageCollector.Server, },
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
}