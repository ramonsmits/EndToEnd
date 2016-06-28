#if Version6
using System.Threading.Tasks;
using NServiceBus;

partial class SagaInitiateRunner
{
    public class TheSaga : Saga<SagaData>,
        IAmStartedByMessages<Command>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
            mapper.ConfigureMapping<Command>(m => m.Identifier).ToSaga(s => s.UniqueIdentifier);
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