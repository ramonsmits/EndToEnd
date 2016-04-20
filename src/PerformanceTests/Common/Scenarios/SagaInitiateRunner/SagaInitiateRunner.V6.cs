#if Version6
using System.Threading;
using System.Threading.Tasks;
using Common.Scenarios;
using NServiceBus;

partial class SagaInitiateRunner : ICreateSeedData
{
    int messageId = 0;

    public int SeedSize { get; set; } = 10000;

    public Task SendMessage(IEndpointInstance endpointInstance)
    {
        return endpointInstance.SendLocal(new Command(Interlocked.Increment(ref messageId)));
    }

    public class TheSaga : Saga<SagaData>,
        IAmStartedByMessages<Command>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
        }

        public Task Handle(Command message, IMessageHandlerContext context)
        {
            Data.UniqueIdentifier = message.Identifier;
            return Task.FromResult(0);
        }
    }

    public class SagaData : ContainSagaData
    {
        public virtual int UniqueIdentifier { get; set; }
    }
}
#endif