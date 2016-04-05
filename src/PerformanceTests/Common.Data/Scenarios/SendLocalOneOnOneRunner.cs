using System;
using System.Threading.Tasks;
using NServiceBus;

partial class SendLocalOneOnOneRunner : IStartAndStop
{
    const int seedSize = 1000;

    public void Start()
    {
        var po = new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount - 1 // Leave one core for transport and persistence
        };

        Parallel.For(0, seedSize, po, i =>
        {
            SendLocal(new Command());
        });
    }

    public void Stop()
    {
        Handler.Shutdown = true;

        // Wait until no new messages are processed so queues will be empty.
        long current = 0;
        while (current < (current = System.Threading.Interlocked.Read(ref Handler.Count)))
        {
            System.Threading.Thread.Sleep(250);
        }
    }

    public class Command : ICommand
    {
    }

    partial class Handler
    {
        public static bool Shutdown;
        public static long Count;
    }
}
