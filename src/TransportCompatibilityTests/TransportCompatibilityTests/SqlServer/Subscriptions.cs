namespace TransportCompatibilityTests.SqlServer
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using TransportCompatibilityTests.Common;
    using TransportCompatibilityTests.Common.Messages;
    using TransportCompatibilityTests.Common.SqlServer;

    [TestFixture]
    public class Subscriptions : SqlServerContext
    {
        SqlServerEndpointDefinition subscriberDefinition;
        SqlServerEndpointDefinition publisherDefinition;

        [SetUp]
        public void SetUp()
        {
            subscriberDefinition = new SqlServerEndpointDefinition
            {
                Name = "Subscriber"
            };
            publisherDefinition = new SqlServerEndpointDefinition
            {
                Name = "Publisher"
            };
        }

        [Category("SqlServer")]
        [Test]
        public void It_is_possible_to()
        {
            subscriberDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestEvent),
                    TransportAddress = publisherDefinition.TransportAddressForVersion(3)
                }
            };

            using (var publisher = EndpointFacadeBuilder.CreateAndConfigure(publisherDefinition, 3))
            using (var subscriber = EndpointFacadeBuilder.CreateAndConfigure(subscriberDefinition, 2))
            {
                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => publisher.NumberOfSubscriptions > 0);

                var eventId = Guid.NewGuid();

                publisher.PublishEvent(eventId);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => subscriber.ReceivedEventIds.Any(ei => ei == eventId));
                Assert.AreEqual(1, subscriber.ReceivedEventIds.Length);
            }

            // Let's upgrade

            subscriberDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestEvent),
                    TransportAddress = publisherDefinition.TransportAddressForVersion(3)
                }
            };

            using (var publisher = EndpointFacadeBuilder.CreateAndConfigure(publisherDefinition, 3))
            using (var subscriber = EndpointFacadeBuilder.CreateAndConfigure(subscriberDefinition, 3))
            {
                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => publisher.NumberOfSubscriptions > 1);

                var eventId = Guid.NewGuid();

                publisher.PublishEvent(eventId);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => subscriber.ReceivedEventIds.Length > 1);
                Assert.AreEqual(1, subscriber.ReceivedEventIds.Length);
            }
        }
    }
}