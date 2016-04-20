#if Version6
using System.Threading;
using System.Threading.Tasks;
using Common.Scenarios;
using NServiceBus;

partial class SagaInitiateRunner : ICreateSeedData
{
    int messageId = 0;

    public int SeedSize { get; set; } = 10000;

    public async Task SendMessage(IEndpointInstance endpointInstance)
    {
        await endpointInstance.SendLocal(new Command(Interlocked.Increment(ref messageId)));
    }

    public class TheSaga : Saga<SagaData>,
        IAmStartedByMessages<Command>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
        }

        public async Task Handle(Command message, IMessageHandlerContext context)
        {
            Data.UniqueIdentifier = message.Identifier;
        }
    }

    public class SagaData : ContainSagaData
    {
        public virtual int UniqueIdentifier { get; set; }
    }
}
#endif