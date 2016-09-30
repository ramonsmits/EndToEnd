using System.Threading.Tasks;
using NServiceBus;

partial class SagaCongestionRunner
    : BaseRunner
{
    protected override Task Start(ISession session)
    {
        var seedSize = MaxConcurrencyLevel * Permutation.PrefetchMultiplier * 2;
        return Zeed(SeedWindow, seedSize, i => session.SendLocal(new Command { Data = Data }));
    }

    public class Command : ICommand
    {
        public int Identifier { get; set; }
        public byte[] Data { get; set; }
    }
}
