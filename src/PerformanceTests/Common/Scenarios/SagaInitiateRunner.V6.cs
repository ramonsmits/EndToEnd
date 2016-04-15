#if Version6
using System.Threading.Tasks;
using Common.Scenarios;
using NServiceBus;

partial class SagaInitiateRunner : ICreateSeedData
{
    int messageId = 0;
    object lockable = new object();

    public int SeedSize { get; set; } = 10000;

    public void SendMessage(IEndpointInstance endpointInstance, string endpointName)
    {
        lock (lockable)
        {
            messageId++;
            endpointInstance.SendLocal(new Command(messageId));
        }
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