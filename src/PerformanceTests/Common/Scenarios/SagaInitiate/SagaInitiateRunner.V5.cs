#if Version5
using NServiceBus.Saga;

partial class SagaInitiateRunner
{
    public class TheSaga : Saga<SagaData>,
        IAmStartedByMessages<Command>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
            mapper.ConfigureMapping<Command>(m => m.Identifier).ToSaga(s => s.UniqueIdentifier);
        }

        public void Handle(Command message)
        {
            Data.UniqueIdentifier = message.Identifier;
        }
    }

    public class SagaData : ContainSagaData
    {
        [Unique]
        public virtual int UniqueIdentifier { get; set; }
    }
}
#endif