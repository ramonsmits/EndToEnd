using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

/// <summary>
/// Performs a continious test where a batch of messages is send via the bus without
/// a transaction and a handler processes these in parallel. Once all messages are
/// received it repeats this. Due to the fact that the sending is not transactional
/// the handler will already process messages while the batch is still being send.
/// </summary>
partial class GatedSendLocalRunner : LoopRunner
{
    protected override async Task SendMessage()
    {
        await SendLocal(CommandGenerator.Create());
    }

    static class CommandGenerator
    {
        static long orderId;

        public static Command Create()
        {
            return new Command
            {
                OrderId = Interlocked.Increment(ref orderId).ToString(CultureInfo.InvariantCulture)
            };
        }
    }

    public class Command : ICommand
    {
        public string OrderId { get; set; }
        public decimal Value { get; set; }
    }
}
