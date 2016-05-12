using System.Threading.Tasks;
using NServiceBus;

/// <summary>
/// Does a continious test where a configured set of messages are 'seeded' on the
/// queue. For each message that is received one message will be send. This means
/// that the sending of the message is part of the receiving context and thus
/// part of the same transaction.
/// 
/// Then the test is stopped the handler stops forwarding the message. The test
/// waits until no new messages are received.
/// </summary>
partial class SendLocalOneOnOneRunner : BaseRunner
{
    const int seedSize = 1000;

    protected override Task Start(ISession session)
    {
        return TaskHelper.ParallelFor(seedSize, () => session.SendLocal(new Command { Data = Data }));
    }

    protected override Task Stop()
    {
        Handler.Shutdown = true;
        return Task.FromResult(0);
    }

    public class Command : ICommand
    {
        public byte[] Data { get; set; }
    }

    partial class Handler
    {
        public static bool Shutdown;
        public static long Count;
    }
}
