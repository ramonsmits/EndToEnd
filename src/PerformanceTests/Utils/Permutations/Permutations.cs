namespace Tests.Permutations
{
    using System;
    using Variables;

    public class Permutations
    {
        public Audit[] AuditModes = { Audit.Off };
        public MessageSize[] MessageSizes = { MessageSize.Tiny };
        public NServiceBusVersion[] Versions = (NServiceBusVersion[])Enum.GetValues(typeof(NServiceBusVersion));
        public Outbox[] OutboxModes;
        public Persistence[] Persisters = { Persistence.InMemory };
        public Platform[] Platforms = { Platform.x64 };
        public Serialization[] Serializers = { Serialization.Json, };
        public Transport[] Transports = (Transport[])Enum.GetValues(typeof(Transport));
        public GarbageCollector[] GarbageCollectors = { GarbageCollector.Server };
        public TransactionMode[] TransactionMode = { Variables.TransactionMode.Transactional };
        public ConcurrencyLevel[] ConcurrencyLevels = { ConcurrencyLevel.EnvCores08x };
        public ScaleOut[] ScaleOuts = { ScaleOut.NoScaleOut };
    }
}
