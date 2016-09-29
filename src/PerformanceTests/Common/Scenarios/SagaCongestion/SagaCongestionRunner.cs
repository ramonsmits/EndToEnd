using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

partial class SagaCongestionRunner
    : BaseRunner
{
    readonly ILog Log = LogManager.GetLogger(nameof(SagaCongestionRunner));

    protected override Task Start(ISession session)
    {
        var seedSize = MaxConcurrencyLevel * Permutation.PrefetchMultiplier * 2;
        Log.InfoFormat("Seeding {0} messages based on concurrency level of {1}.", seedSize, MaxConcurrencyLevel);
        return BatchHelper.Batch(seedSize,
            i =>
            {
                Log.InfoFormat("Index: {0}", i);
                return session.SendLocal(new Command
                {
                    Identifier = 1,
                    Data = Data
                });
            });
    }

    public class Command : ICommand
    {
        public int Identifier { get; set; }
        public byte[] Data { get; set; }
    }
}
