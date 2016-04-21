using System.Threading;
using System.Threading.Tasks;
using Common.Scenarios;
using NServiceBus;

partial class SagaInitiateRunner : BaseRunner, ICreateSeedData
{
    int messageId;

    public int SeedSize { get; } = 10000;

    public Task SendMessage(ISession session)
    {
        return session.SendLocal(new Command(Interlocked.Increment(ref messageId)));
    }

    public class Command : ICommand
    {
        public Command(int id)
        {
            Identifier = id;
        }

        public int Identifier { get; set; }
    }
}
