#if Version5
using NServiceBus.Saga;

partial class SagaInitiateRunner
{
    public class TheSaga : Saga<SagaData>,
        IAmStartedByMessages<Command>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
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