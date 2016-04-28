namespace Categories
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Variables;

    [TestFixture(Description = "MongoDB", Category = "Community")]
    public class MongoDBFixture : Base
    {
        [TestCaseSource(nameof(CreatePermutations))]
        public override void GatedPublishRunner(Permutation permutation)
        {
            base.GatedPublishRunner(permutation);
        }

        [TestCaseSource(nameof(CreatePermutations))]
        public override void SagaInitiateRunner(Permutation permutation)
        {
            base.SagaInitiateRunner(permutation);
        }

        static IEnumerable<Permutation> CreatePermutations()
        {
            return PermutationGenerator.Generate(new Permutations
            {
                Versions = new[] { NServiceBusVersion.V5 },
                Transports = new[] { Transport.MSMQ },
                Persisters = new[] { Persistence.MongoDB },
                Serializers = new[] { Serialization.Json },
                OutboxModes = new[] { Outbox.Off, Outbox.On },
                DTCModes = new[] { DTC.Off }
            });
        }
    }
}
