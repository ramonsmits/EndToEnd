namespace Categories
{
    using Variables;

    public class Permutation
    {
        public string[] Tests =
        {
            "PublishToSelf",
            "SendLocal",
            "SendToSelf"
        };

        public Variables.Audit AuditMode; //0
        public Variables.DTC DTCMode; //1
        public Variables.IOPS IOPS;//2
        public Variables.MessageSize MessageSize;//3
        public Variables.NServiceBusVersion Version;//4
        public Variables.Outbox OutboxMode;//5
        public Variables.Persistence Persister;//6
        public Variables.Platform Platform;//7
        public Variables.Serialization Serializer;//8
        public Variables.Transport Transport;//9
        public Variables.GarbageCollector GarbageCollector;//10
        public Variables.TransactionMode TransactionMode;//11
        public ConcurrencyLevel ConcurrencyLevel;//12

        public override string ToString()
        {
            return $"{AuditMode};{DTCMode};{IOPS};{MessageSize};{Version};{OutboxMode};{Persister};{Platform};{Serializer};{Transport};{GarbageCollector};{TransactionMode};{ConcurrencyLevel}";
        }
    }
}