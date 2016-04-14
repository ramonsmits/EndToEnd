#if Version5

using System;
using System.Threading;
using Common.Scenarios;
using NServiceBus;
using NServiceBus.Saga;

partial class SagaInitiateRunner : ICreateSeedData
{
    public int SeedSize { get; set; } = 10000;
    int messageId = 1;

    public void SendMessage(ISendOnlyBus endpointInstance, string endpointName)
    {
        var address = new Address(endpointName, "localhost");
        endpointInstance.Send(address, new Command(messageId));
        Interlocked.Increment(ref messageId);
    }

    public class TheSaga : Saga<SagaData>,
        IAmStartedByMessages<Command>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaData> mapper)
        {
            mapper.ConfigureMapping<Command>(message => message.Id).ToSaga(saga => saga.UniqueIdentifier);
        }

        public void Handle(Command message)
        {
        }
    }

    public class SagaData : IContainSagaData
    {
        Guid IContainSagaData.Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        [Unique]
        public int UniqueIdentifier { get; set; }
    }
}
#endif