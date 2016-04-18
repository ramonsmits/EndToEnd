using System;
using System.Diagnostics;
using NServiceBus;
using NServiceBus.Logging;
using Tests.Permutations;
using Variables;

class MsmqProfile : IProfile, INeedPermutation
{
    ILog Log = LogManager.GetLogger(nameof(MsmqProfile));

    public Permutation Permutation { private get; set; }

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        transport.ConnectionString(this.GetConnectionString("MSMQ"));

        if (Permutation.DTCMode == DTC.Off)
            transport.Transactions(TransportTransactionMode.ReceiveOnly | TransportTransactionMode.SendsAtomicWithReceive);

        RunInspections();
    }

    long SizeThresshold = 1024 * 1024 * 1024; // 1GB
    long CountThresshold = 100000;

    void RunInspections()
    {
        try
        {
            long size, count;

            using (var bytes = new PerformanceCounter("MSMQ Service", "Total bytes in all queues"))
            {
                size = bytes.RawValue;
            }

            using (var messages = new PerformanceCounter("MSMQ Service", "Total messages in all queues"))
            {
                count = messages.RawValue;
            }

            Log.InfoFormat("MSMQ Currently contains {0:N0} messages, occupying {1:N0} bytes", count, size);

            if (count > CountThresshold || size > SizeThresshold)
            {
                Log.WarnFormat("MSMQ message count ({0:N0}) or size ({1:N0}) exceeded. Please verify if MSMQ has a lot of (journaled) messages or message in the system (transactional) dead letter queue.", CountThresshold, SizeThresshold);
            }
        }
        catch (Exception ex)
        {
            Log.Debug("Optional MSMQ inspections failed to run.", ex);
        }
    }
}
