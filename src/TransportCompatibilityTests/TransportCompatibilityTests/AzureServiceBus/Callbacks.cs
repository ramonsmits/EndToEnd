﻿namespace TransportCompatibilityTests.AzureServiceBus
{
    using System.Linq;
    using NUnit.Framework;
    using TransportCompatibilityTests.Common;
    using TransportCompatibilityTests.Common.Messages;
    using TransportCompatibilityTests.Common.AzureServiceBus;

    [TestFixture]
    public class Callbacks
    {
        private AzureServiceBusEndpointDefinition sourceEndpointDefinition;
        private AzureServiceBusEndpointDefinition destinationEndpointDefinition;

        [SetUp]
        public void SetUp()
        {
            sourceEndpointDefinition = new AzureServiceBusEndpointDefinition { Name = "Source" };
            destinationEndpointDefinition = new AzureServiceBusEndpointDefinition { Name = "Destination" };
        }

        [Category("AzureServiceBus")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        // ReSharper disable once InconsistentNaming
        public void Int_callbacks_work(int sourceVersion, int destinationVersion)
        {
            this.sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestIntCallback),
                    TransportAddress = destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(sourceEndpointDefinition, sourceVersion))
            using (EndpointFacadeBuilder.CreateAndConfigure(destinationEndpointDefinition, destinationVersion))
            {
                var value = 42;

                source.SendAndCallbackForInt(value);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => source.ReceivedIntCallbacks.Contains(value));
            }
        }

        [Category("AzureServiceBus")]
        [Test, TestCaseSource(nameof(GenerateVersionsPairs))]
        // ReSharper disable once InconsistentNaming
        public void Enum_callbacks_work(int sourceVersion, int destinationVersion)
        {
            sourceEndpointDefinition.Mappings = new[]
            {
                new MessageMapping
                {
                    MessageType = typeof(TestEnumCallback),
                    TransportAddress = this.destinationEndpointDefinition.TransportAddressForVersion(destinationVersion)
                }
            };

            using (var source = EndpointFacadeBuilder.CreateAndConfigure(sourceEndpointDefinition, sourceVersion))
            using (EndpointFacadeBuilder.CreateAndConfigure(destinationEndpointDefinition, destinationVersion))
            {
                var value = CallbackEnum.Three;

                source.SendAndCallbackForEnum(value);

                // ReSharper disable once AccessToDisposedClosure
                AssertEx.WaitUntilIsTrue(() => source.ReceivedEnumCallbacks.Contains(value));
            }
        }

        private static object[][] GenerateVersionsPairs()
        {
            var asbTransportVersions = new[] { 6, 7 };

            var pairs = from l in asbTransportVersions
                        from r in asbTransportVersions
                        select new object[] { l, r };

            return pairs.ToArray();
        }
    }
}