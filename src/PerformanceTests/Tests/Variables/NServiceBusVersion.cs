enum NServiceBusVersion
{
    V5,
    V6
}

enum IOPS
{
    Slow, // HDD
    Normal, // SSD
    Fast // SSD M2
}


enum Platform
{
    x86,
    x64
}

enum gcServer
{
    On,
    Off
}

enum Transport
{
    MSMQ,
    RMQ,
    ASQ,
    ASB,
}

enum Persistence
{
    InMemory,
    NHibernate,
    RavenDB,
    MSMQ
}

enum Serialization
{
    Json,
    Xml
}

enum MessageSize
{
    Tiny,
    Small,
    Medium,
    Large
}

enum Outbox
{
    On,
    Off
}

enum DTC
{
    On,
    Off
}

enum TransactionMode
{
    Receive,
    Unreliable,
    AtomicSends
}

enum Audit
{
    On,
    Off
}
