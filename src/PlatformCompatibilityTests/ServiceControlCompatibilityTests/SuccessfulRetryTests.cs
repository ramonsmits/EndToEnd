﻿namespace ServiceControlCompatibilityTests
{
    using NServiceBus;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    class SuccessfulRetryTests : SqlScTest
    {
        [TestCaseSource(nameof(AllTransports))]
        public Task Can_successfully_retry_a_failed_message(Type transportDetailType)
        {
            var endpointFactory = StartUp(transportDetailType);
            return RunTest(endpointFactory);
        }

        async Task RunTest(IEndpointFactory endpointFactory)
        {
            var failingMessageId = Guid.NewGuid().ToString();
            var testContext = new TestContext();

            var sender = await endpointFactory.CreateEndpoint("Sender");
            var processor = await endpointFactory.CreateEndpoint("Processor", new EndpointDetails().With<TestMessageHandler>().With(testContext));

            testContext.ShouldFail = true;
            await sender.Send(processor, new TestMessage(), failingMessageId);

            var failedMessage = await ServiceControl.WaitForFailedMessage(failingMessageId);

            testContext.ShouldFail = false;
            await ServiceControl.RetryMessageId(failedMessage.Id);

            var handled = await testContext.WaitForDone();

            Assert.IsTrue(handled, "Did not process the retry successfully within the time limit");
        }
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