using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

partial class SagaUpdateRunner
    : BaseRunner
{
    readonly ILog Log = LogManager.GetLogger(nameof(SagaUpdateRunner));

    protected override Task Start(ISession session)
    {
        var seedSize = MaxConcurrencyLevel * 2;
        Log.InfoFormat("Seeding {0} messages based on concurrency level of {1}.", seedSize, MaxConcurrencyLevel);
        return BatchHelper.Instance.Batch(seedSize,
            i =>
            {
                Log.InfoFormat("Index: {0}", i);
                return session.SendLocal(new Command
                {
                    Identifier = ++i, // Workaround for V6 as it doesn't allow default values as key
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
