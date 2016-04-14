namespace TransportCompatibilityTests.RabbitMQ
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using TransportCompatibilityTests.Common;
    using TransportCompatibilityTests.Common.Messages;
    using TransportCompatibilityTests.Common.RabbitMQ;
    using TransportCompatibilityTests.RabbitMQ.Infrastructure;

    [TestFixture]
    public class MessageExchangePatterns : RabbitMqContext
    {
        private RabbitMqEndpointDefinition sourceEndpointDefinition;
        private RabbitMqEndpointDefinition destinationEndpointDefinition;

        [SetUp]
        public void SetUp()
        {
            this.sourceEndpointDefinition = new RabbitMqEndpointDefinition { Name = "src" };
            this.destinationEndpointDefinition = new RabbitMqEndpointDefinition { Name = "dst" };
        }

        [Category("RabbitMQ")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        public void It_is_possible_to_send_command_between_different_versions(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Mappings = new []
            {
                new MessageMapping
                {
                    MessageType = typeof(TestCommand),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            using (var destination = EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
            {
                var messageId = Guid.NewGuid();

                source.SendCommand(messageId);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => destination.ReceivedMessageIds.Any(mi => mi == messageId));
            }
        }

        [Category("RabbitMQ")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        public void It_is_possible_to_publish_events(int sourceVersion, int destinationVersion)
        {

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            using (var destination = EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
            {
                var eventId = Guid.NewGuid();

                source.PublishEvent(eventId);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => destination.ReceivedEventIds.Any(ei => ei == eventId));
            }
        }

        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        public void It_is_possible_to_send_request_and_receive_replay(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestRequest),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            using (EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
            {
                var requestId = Guid.NewGuid();

                source.SendRequest(requestId);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => source.ReceivedResponseIds.Any(responseId => responseId == requestId));
            }
        }

        private static object[][] GenerateVersionsPairs()
        {
            var versions = new[] { 1, 2, 3 };
            //var versions = new[] { 2, 3 };

            var pairs = from l in versions
                        from r in versions
                        where l != r
                        select new object[] { l, r };

            return pairs.ToArray();
        }
    }

    
}
