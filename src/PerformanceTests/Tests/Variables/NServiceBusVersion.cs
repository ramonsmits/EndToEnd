namespace Variables
{
    public enum NServiceBusVersion
    {
        V5,
        V6
    }

    public enum IOPS
    {
        Slow, // HDD
        Normal, // SSD
        Fast // SSD M2
    }


    public enum Platform
    {
        x86,
        x64
    }

    public enum GarbageCollector
    {
        Client,
        Server
    }

    public enum Transport
    {
        MSMQ,
        RMQ,
        ASQ,
        ASB,
        SQL
    }

    public enum Persistence
    {
        InMemory,
        NHibernate,
        RavenDB,
        //MSMQ
    }

    public enum Serialization
    {
        Json,
        Xml
    }

    public enum MessageSize
    {
        Tiny,
        Small,
        Medium,
        Large
    }

    public enum Outbox
    {
        On,
        Off
    }

    public enum DTC
    {
        On,
        Off
    }

    public enum TransactionMode
    {
        Default,
        Receive,
        Unreliable,
        AtomicSends
    }

    public enum Audit
    {
        On,
        Off
    }

    public enum ConcurrencyLevel
    {
        Sequential,
        EnvCores,
        EnvCores4x
    }
}
