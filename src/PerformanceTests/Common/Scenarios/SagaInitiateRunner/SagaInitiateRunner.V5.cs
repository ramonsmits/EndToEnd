#if Version5
using System;
using System.Threading;
using Common.Scenarios;
using NServiceBus;
using NServiceBus.Saga;

partial class SagaInitiateRunner : ICreateSeedData
{
    Address address;
    int messageId;

    public int SeedSize { get; set; } = 10000;

    public void SendMessage(ISendOnlyBus endpointInstance)
    {
        if (address == null)
        {
            var unicastBus = (NServiceBus.Unicast.UnicastBus)endpointInstance;
            var machine = unicastBus.Configure.LocalAddress.Machine;
            var queue = unicastBus.Configure.LocalAddress.Queue;

            address = new Address(queue, machine);
        }

        endpointInstance.Send(address, new Command(Interlocked.Increment(ref messageId)));
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