using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

partial class SagaUpdateRunner
    : BaseRunner
{
    readonly ILog Log = LogManager.GetLogger(nameof(SagaUpdateRunner));

    protected override async Task Start(ISession session)
    {
        var seedSize = MaxConcurrencyLevel * 2;
        Log.InfoFormat("Seeding {0} messages based on concurrency level of {1}.", seedSize, MaxConcurrencyLevel);
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

    protected override Task Stop()
    {
        return DrainMessages();
    }

    public class Command : ICommand
    {
        public int Identifier { get; set; }
        public byte[] Data { get; set; }
    }
}
