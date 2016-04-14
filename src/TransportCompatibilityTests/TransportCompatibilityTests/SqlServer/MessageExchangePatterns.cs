using System;
using System.Linq;
using NUnit.Framework;
using TransportCompatibilityTests.Common;
using TransportCompatibilityTests.Common.Messages;

// ReSharper disable InconsistentNaming

namespace TransportCompatibilityTests.SqlServer
{
    using TransportCompatibilityTests.Common.SqlServer;
    using MessageMapping = TransportCompatibilityTests.Common.SqlServer.MessageMapping;

    [TestFixture]
    public class MessageExchangePatterns : SqlServerContext
    {
        SqlServerEndpointDefinition sourceEndpointDefinition;
        SqlServerEndpointDefinition destinationEndpointDefinition;

        [SetUp]
        public void SetUp()
        {
            this.sourceEndpointDefinition = new SqlServerEndpointDefinition
            {
                Name = "Source"
            };
            this.destinationEndpointDefinition = new SqlServerEndpointDefinition
            {
                Name = "Destination"
            };
        }

        [Category("SqlServer")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        public void It_is_possible_to_send_command_between_different_versions(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestCommand),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            {
                using (var destination = EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
                {
                    var messageId = Guid.NewGuid();

                    source.SendCommand(messageId);

                    // ReSharper disable once AccessToDisposedClosure
                    AssertEx.WaitUntilIsTrue(() => destination.ReceivedMessageIds.Any(mi => mi == messageId));
                }
            }
        }

        [Category("SqlServer")]
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
            {
                using (EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
                {
                    var requestId = Guid.NewGuid();

                    source.SendRequest(requestId);

                    // ReSharper disable once AccessToDisposedClosure
                    AssertEx.WaitUntilIsTrue(() => source.ReceivedResponseIds.Any(responseId => responseId == requestId));
                }
            }
        }

        [Category("SqlServer")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        public void It_is_possible_to_publish_events(int sourceVersion, int destinationVersion)
        {
            this.destinationEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestEvent),
                    TransportAddress = this.sourceEndpointDefinition.TransportAddressForVersion(sourceVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            {
                using (var destination = EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
                {
                    // ReSharper disable once AccessToDisposedClosure
                    AssertEx.WaitUntilIsTrue(() => source.NumberOfSubscriptions > 0);

                    var eventId = Guid.NewGuid();

                    source.PublishEvent(eventId);

                    // ReSharper disable once AccessToDisposedClosure
                    AssertEx.WaitUntilIsTrue(() => destination.ReceivedEventIds.Any(ei => ei == eventId));
                }
            }
        }

        [Category("SqlServer")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        public void It_is_possible_to_send_and_receive_using_custom_schema_in_transport_address(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Schema = SourceSchema;
            this.sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestRequest),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion),
                    Schema = DestinationSchema
                }
            };

            this.destinationEndpointDefinition.Schema = DestinationSchema;

            //TODO: this is a hack, passing mappings should be separate from passing schemas
            if (sourceVersion == 2 && destinationVersion == 3)
            {
                this.destinationEndpointDefinition.Mappings = new[]
                {
                    new MessageMapping
                    {
                        MessageType = typeof(TestResponse),
                        TransportAddress = this.sourceEndpointDefinition.TransportAddressForVersion(sourceVersion) + "." + Environment.MachineName,
                        Schema = SourceSchema
                    }
                };
            }
            else
            {
                this.destinationEndpointDefinition.Mappings = new[]
                {
                    new MessageMapping
                    {
                        MessageType = typeof(TestResponse),
                        TransportAddress = this.sourceEndpointDefinition.TransportAddressForVersion(sourceVersion),
                        Schema = SourceSchema
                    }
                };
            }

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            {
                using (EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
                {
                    var requestId = Guid.NewGuid();

                    source.SendRequest(requestId);

                    // ReSharper disable once AccessToDisposedClosure
                    AssertEx.WaitUntilIsTrue(() => source.ReceivedResponseIds.Any());
                }
            }
        }

        static object[][] GenerateVersionsPairs()
        {
            var sqlTransportVersions = new[]
            {
                1,
                2,
                3
            };

            var pairs = from l in sqlTransportVersions
                from r in sqlTransportVersions
                where l != r
                select new object[]
                {
                    l,
                    r
                };

            return pairs.ToArray();
        }
    }
}