namespace Categories
{
    using System.Collections.Generic;
    using Variables;

    public class Permutation
    {
        public ICollection<Package> Packages { get; set; }
        public List<string> Tests = new List<string>
        {
            "PublishToSelf",
            "SendLocal",
            "SendToSelf"
        };

        public Variables.Audit AuditMode;
        public Variables.DTC DTCMode;
        public Variables.IOPS IOPS;
        public Variables.MessageSize MessageSize;
        public Variables.NServiceBusVersion Version;
        public Variables.Outbox OutboxMode;
        public Variables.Persistence Persister;
        public Variables.Platform Platform;
        public Variables.Serialization Serializer;
        public Variables.Transport Transport;
        public Variables.GarbageCollector GarbageCollector;
        public Variables.TransactionMode TransactionMode;
        public ConcurrencyLevel ConcurrencyLevel;

        public override string ToString()
        {
            return $"{AuditMode};{DTCMode};{IOPS};{MessageSize};{Version};{OutboxMode};{Persister};{Platform};{Serializer};{Transport};{GarbageCollector};{TransactionMode};{ConcurrencyLevel}";
        }

        public class Package
        {
            string Name;
            string PackageVersion;
        }
    }
}