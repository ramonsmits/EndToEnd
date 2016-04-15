namespace RabbitMQV2
{
    using NServiceBus;
    using TransportCompatibilityTests.Common;
    using TransportCompatibilityTests.Common.Messages;

    public class Handler : IHandleMessages<TestCommand>,
        IHandleMessages<TestEvent>,
        IHandleMessages<TestRequest>, IHandleMessages<TestResponse>
    {
        public IBus Bus { get; set; }
        public MessageStore Store { get; set; }

        public void Handle(TestCommand command)
        {
            Store.Add<TestCommand>(command.Id);
        }

        public void Handle(TestEvent message)
        {
            Store.Add<TestEvent>(message.EventId);
        }

        public void Handle(TestRequest message)
        {
            Bus.Reply(new TestResponse { ResponseId = message.RequestId });
        }

        public void Handle(TestResponse message)
        {
            Store.Add<TestResponse>(message.ResponseId);
        }
    }
}
