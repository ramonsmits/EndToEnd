namespace Categories
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Tests.Permutations;
    using Variables;

    [TestFixture(Description = "Concurrency", Category = "Performance"), Explicit]
    public class ConcurrencyFixture : Base
    {
        [TestCaseSource(nameof(CreatePermutations))]
        public override void ReceiveRunner(Permutation permutation)
        {
            base.ReceiveRunner(permutation);
        }

        [TestCaseSource(nameof(CreatePermutations))]
        public override void SendLocalOneOnOneRunner(Permutation permutation)
        {
            base.SendLocalOneOnOneRunner(permutation);
        }

        static IEnumerable<Permutation> CreatePermutations()
        {
            return PermutationGenerator.Generate(new Permutations
            {
                Transports = new[] { Transport.MSMQ, Transport.RabbitMQ, }, //2
                Serializers = new[] { Serialization.Json, },
                OutboxModes = new[] { Outbox.Off, },
                TransactionMode = new[] { TransactionMode.Receive, TransactionMode.Atomic, TransactionMode.Transactional, TransactionMode.None },  //4
                ConcurrencyLevels = new[] { ConcurrencyLevel.EnvCores04x, ConcurrencyLevel.EnvCores08x, ConcurrencyLevel.EnvCores16x, } //3 => 2*4*3 * 2m = 48m
            });
        }
    }
}