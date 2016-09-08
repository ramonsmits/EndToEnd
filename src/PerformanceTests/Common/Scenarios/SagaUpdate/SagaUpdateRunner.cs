using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

partial class SagaUpdateRunner
    : BaseRunner
{
    public static bool Shutdown;
    readonly ILog Log = LogManager.GetLogger(nameof(SagaUpdateRunner));

    protected override async Task Start(ISession session)
    {
        var maxConcurrencyLevel = ConcurrencyLevelConverter.Convert(Permutation.ConcurrencyLevel);
        var seedSize = maxConcurrencyLevel * 2;
        Log.InfoFormat("Seeding {0} messages based on concurrency level of {1}.", seedSize, maxConcurrencyLevel);
        await TaskHelper.ParallelFor(seedSize,
            i =>
            {
                Log.InfoFormat("Index: {0}", i);
                return session.SendLocal(new Command
                {
                    Identifier = ++i, // Workaround for V6 as it doesn't allow default values as key
                    Data = Data
                });
            }).ConfigureAwait(false);
    }

    protected override async Task Stop()
    {
        Shutdown = true;

        long startCount = Statistics.Instance.NumberOfMessages;
        long current;

        Log.Info("Draining queue...");
        do
        {
            current = Statistics.Instance.NumberOfMessages;
            Log.Debug("Delaying to detect receive activity...");
            await Task.Delay(100).ConfigureAwait(false);
        } while (Statistics.Instance.NumberOfMessages > current);

        var diff = current - startCount;
        Log.InfoFormat("Drained {0} message(s)",diff);
    }

    public class Command : ICommand
    {
        public int Identifier { get; set; }
        public byte[] Data { get; set; }
    }
}
