namespace TransportCompatibilityTests.SqlServer
{
    using System.Linq;
    using NUnit.Framework;
    using TransportCompatibilityTests.Common;
    using TransportCompatibilityTests.Common.Messages;
    using TransportCompatibilityTests.Common.SqlServer;

    [TestFixture]
    public class Callbacks
    {
        private SqlServerEndpointDefinition sourceEndpointDefinition;
        private SqlServerEndpointDefinition destinationEndpointDefinition;

        [SetUp]
        public void SetUp()
        {
            this.sourceEndpointDefinition = new SqlServerEndpointDefinition { Name = "Source" };
            this.destinationEndpointDefinition = new SqlServerEndpointDefinition { Name = "Destination" };
        }

        [Category("SqlServer")]
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

        [Category("SqlServer")]
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
            var sqlTransportVersions = new[] { 1, 2, 3 };

            var pairs = from l in sqlTransportVersions
                        from r in sqlTransportVersions
                        select new object[] { l, r };

            return pairs.ToArray();
        }
    }
}
