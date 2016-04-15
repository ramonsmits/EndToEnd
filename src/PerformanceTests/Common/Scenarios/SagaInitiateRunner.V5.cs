#if Version5
using System;
using Common.Scenarios;
using NServiceBus;
using NServiceBus.Saga;

partial class SagaInitiateRunner : ICreateSeedData
{
    Address address;
    int messageId = 0;
    object lockable = new object();

    public int SeedSize { get; set; } = 10000;

    public void SendMessage(ISendOnlyBus endpointInstance, string endpointName)
    {
        if (address == null)
            address = new Address(endpointName, "localhost");

        lock (lockable)
        {
            messageId++;
            endpointInstance.Send(address, new Command(messageId));
        }
    }

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

    public class SagaData : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }

        [Unique]
        public virtual int UniqueIdentifier { get; set; }
    }
}
#endif