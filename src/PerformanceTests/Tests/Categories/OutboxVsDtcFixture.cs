//namespace Categories
//{
//    using System;
//    using System.Collections.Generic;
//    using NUnit.Framework;
//    using Tests.Permutations;
//    using Variables;

//    [TestFixture(Description = "Outbox vs DTC", Category = "Performance")]
//    public class OutboxVsDtcFixture : Base
//    {
//        [TestCaseSource(nameof(CreatePermutations))]
//        public override void GatedSendLocalRunner(Permutation permutation)
//        {
//            base.GatedSendLocalRunner(permutation);
//        }

//        static IEnumerable<Permutation> CreatePermutations()
//        {
//            return PermutationGenerator.Generate(new Permutations
//            {
//                Transports = new[] { Transport.MSMQ },
//                Persisters = new[] { Persistence.NHibernate, Persistence.RavenDB, },
//                Serializers = new[] { Serialization.Json, },
//                MessageSizes = (MessageSize[])Enum.GetValues(typeof(MessageSize)),
//                OutboxModes = (Outbox[])Enum.GetValues(typeof(Outbox)),
//                DTCModes = (DTC[])Enum.GetValues(typeof(DTC)),
//            });
//        }
//    }
//}