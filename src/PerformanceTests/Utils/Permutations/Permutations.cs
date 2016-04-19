namespace Tests.Permutations
{
    using System;
    using Variables;

    public class Permutations
    {
        public Audit[] AuditModes = { Audit.Off };
        public DTC[] DTCModes = { DTC.On };
        public IOPS[] IOPS = { Variables.IOPS.Default, };
        public MessageSize[] MessageSizes = { MessageSize.Tiny };
        public NServiceBusVersion[] Versions = (NServiceBusVersion[])Enum.GetValues(typeof(NServiceBusVersion));
        public Outbox[] OutboxModes;
        public Persistence[] Persisters = { Persistence.InMemory };
        public Platform[] Platforms = { Platform.x64 };
        public Serialization[] Serializers;
        public Transport[] Transports;
        public GarbageCollector[] GarbageCollectors = { GarbageCollector.Server };
        public TransactionMode[] TransactionMode = { Variables.TransactionMode.Default };
        public ConcurrencyLevel[] ConcurrencyLevels = { ConcurrencyLevel.EnvCores4x, };
        public ScaleOut[] ScaleOuts = { ScaleOut.NoScaleOut };
        public Encryption[] Encryptions = { Encryption.None };
    }
}