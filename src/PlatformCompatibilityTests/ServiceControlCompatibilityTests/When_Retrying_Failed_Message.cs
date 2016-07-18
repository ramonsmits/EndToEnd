namespace ServiceControlCompatibilityTests
{
    using NServiceBus;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    class When_retrying_failed_message : SqlScTest
    {
        [TestCaseSource(nameof(AllTransports))]
        [Timeout(300000)]
        public async Task Gets_routed_back_to_Processing_Endpoint(Type transportDetailType)
        {
            var endpointFactory = await StartUp("Retry", transportDetailType, map =>
            {
                map[SenderEndpointName] = ServerA;
                map[ProcessorEndpointName] = ServerB;
            });
            await RunTest(endpointFactory);
        }

        async Task RunTest(IEndpointFactory endpointFactory)
        {
            var testContext = new TestContext();

            var sender = await endpointFactory.CreateEndpoint(SenderEndpointName);
            Console.WriteLine($"Started {sender}");
            var processor = await endpointFactory.CreateEndpoint(new EndpointDetails(ProcessorEndpointName).With<TestMessageHandler>().With(testContext));
            Console.WriteLine($"Started {processor}");


            Console.WriteLine($"Sending test message from {sender} to {processor} ... should fail.");
            testContext.ShouldFail = true;
            var failingMessageId = await sender.Send(processor, new TestMessage());

            Console.WriteLine($"Waiting for a message with id {failingMessageId} to show up in ServiceControl");
            var failedMessage = await ServiceControl.WaitForFailedMessage(failingMessageId);

            Console.WriteLine($"Found it, retrying now. Should succeed this time");
            testContext.ShouldFail = false;
            await ServiceControl.RetryMessageId(failedMessage.Id);

            Console.WriteLine("And now we wait to see if it worked");
            var handled = await testContext.WaitForDone();

            Assert.IsTrue(handled, "Did not process the retry successfully within the time limit");
        }

        const string SenderEndpointName = "Sender";
        const string ProcessorEndpointName = "Processor";
    }

    class TestMessage : ICommand
    {
    }

    class TestMessageHandler : IHandleMessages<TestMessage>
    {
        readonly TestContext testContext;

        public TestMessageHandler(TestContext testContext)
        {
            this.testContext = testContext;
        }

        public Task Handle(TestMessage message, IMessageHandlerContext context)
        {
            if (testContext.ShouldFail)
            {
                throw new ApplicationException("Failing...");
            }

            testContext.Done(true);
            return Task.FromResult(0);
        }
    }

    class TestContext : TestContextBase
    {
        public bool ShouldFail { get; set; }
    }
}