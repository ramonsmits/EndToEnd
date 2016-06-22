namespace ServiceControlCompatibilityTests
{
    using Autofac;
    using NServiceBus;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    class SuccessfulRetryTests : SqlScTest
    {
        [TestCaseSource(nameof(AllTransports))]
        public async Task Can_successfully_retry_a_failed_message(Type transportDetailType)
        {
            var transportDetails = StartUp(transportDetailType);
            await RunTest(transportDetails);
        }

        async Task RunTest(ITransportDetails transportDetails)
        {
            var handlerInstance = new TestMessageHandler();
            var builder = new ContainerBuilder();
            builder.RegisterInstance(handlerInstance);
            var container = builder.Build();

            var endpointA = await CreateSqlEndpoint("EndpointA", transportDetails, container);
            await CreateSqlEndpoint("EndpointB", transportDetails, container);

            handlerInstance.ShouldFail = true;
            endpointA.Send("EndpointB", new TestMessage());

            var failedMessage = await ServiceControl.WaitForNewFailingMessages("EndpointA");

            handlerInstance.ShouldFail = false;
            await ServiceControl.RetryMessageId(failedMessage.Id);

            var handled = handlerInstance.WaitForSuccessfulMessage().Wait(60000); // Is this a reasonable time to wait?

            Assert.IsTrue(handled, "Did not process the retry successfully within the time limit");
        }
    }

    class TestMessage : ICommand
    {
    }

    class TestMessageHandler : IHandleMessages<TestMessage>
    {
        TaskCompletionSource<bool> successfulMessageSource;

        public TestMessageHandler()
        {
            successfulMessageSource = new TaskCompletionSource<bool>();
        }

        public bool ShouldFail { get; set; }

        public Task Handle(TestMessage message, IMessageHandlerContext context)
        {
            if (ShouldFail)
            {
                throw new ApplicationException("Failing...");
            }

            successfulMessageSource.SetResult(true);
            return Task.FromResult(0);
        }

        public Task WaitForSuccessfulMessage()
        {
            return successfulMessageSource.Task;
        }
    }
}