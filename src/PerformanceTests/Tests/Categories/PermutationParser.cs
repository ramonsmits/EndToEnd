using System;
using Categories;
using Variables;

public static class PermutationParser
{
    static readonly char Seperator = ';';

    public static Permutation FromCommandline()
    {
        return Parse(Environment.CommandLine);
    }

    public static Permutation Parse(string data)
    {
        var values = data.Split(Seperator);

        return new Permutation
        {
            AuditMode = values.Parse<Audit>(0),
            DTCMode = values.Parse<DTC>(1),
            IOPS = values.Parse<IOPS>(2),
            MessageSize = values.Parse<MessageSize>(3),
            Version = values.Parse<NServiceBusVersion>(4),
            OutboxMode = values.Parse<Outbox>(5),
            Persister = values.Parse<Persistence>(6),
            Platform = values.Parse<Platform>(7),
            Serializer = values.Parse<Serialization>(8),
            Transport = values.Parse<Transport>(9),
            GarbageCollector = values.Parse<GarbageCollector>(10),
            TransactionMode = values.Parse<TransactionMode>(11),
            ConcurrencyLevel = values.Parse<ConcurrencyLevel>(12),
        };
    }

    static T Parse<T>(this string[] values, int index)
    {
        return (T)Enum.Parse(typeof(T), values[index]);
    }
}
