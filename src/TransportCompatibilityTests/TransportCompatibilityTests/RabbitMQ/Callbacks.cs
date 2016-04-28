namespace TransportCompatibilityTests.RabbitMQ
{
    using System.Linq;
    using NUnit.Framework;
    using TransportCompatibilityTests.Common;
    using TransportCompatibilityTests.Common.Messages;
    using TransportCompatibilityTests.Common.RabbitMQ;
    using TransportCompatibilityTests.RabbitMQ.Infrastructure;

    [TestFixture]
    public class Callbacks : RabbitMqContext
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
        // ReSharper disable once InconsistentNaming
        public void Int_callbacks_work(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestIntCallback),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            using (EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
            {
                var value = 42;

                source.SendAndCallbackForInt(value);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => source.ReceivedIntCallbacks.Contains(value));
            }
        }

        [Category("RabbitMQ")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        // ReSharper disable once InconsistentNaming
        public void Enum_callbacks_work(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestEnumCallback),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(this.sourceEndpointDefinition, sourceVersion))
            using (EndpointFacadeBuilder.CreateAndConfigure(this.destinationEndpointDefinition, destinationVersion))
            {
                var value = CallbackEnum.Three;

                source.SendAndCallbackForEnum(value);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => source.ReceivedEnumCallbacks.Contains(value));
            }
        }

        private static object[][] GenerateVersionsPairs()
        {
            var versions = new[] { 1, 2, 3, 4 };

            var pairs = from l in versions
                        from r in versions
                        where l != r
                        select new object[] { l, r };

            return pairs.ToArray();
        }
    }
}
