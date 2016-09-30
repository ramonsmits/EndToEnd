using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

partial class SagaUpdateRunner
    : BaseRunner
{
    readonly ILog Log = LogManager.GetLogger(nameof(SagaUpdateRunner));

    protected override Task Start(ISession session)
    {
        var seedSize = MaxConcurrencyLevel * Permutation.PrefetchMultiplier * 2;
        return Zeed(SeedWindow, seedSize, i => session.SendLocal(new Command { Identifier = ++i, Data = Data }));
    }

    public class Command : ICommand
    {
        public int Identifier { get; set; }
        public byte[] Data { get; set; }
    }
}
