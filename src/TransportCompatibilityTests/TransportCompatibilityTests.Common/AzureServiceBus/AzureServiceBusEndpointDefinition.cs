namespace TransportCompatibilityTests.Common.AzureServiceBus
{
    using System;

    [Serializable]
    public class AzureServiceBusEndpointDefinition : EndpointDefinition
    {
        public override string TransportName => "AzureServiceBus";
        public MessageMapping[] Mappings { get; set; }

        public AzureServiceBusEndpointDefinition()
        {
            Mappings = new MessageMapping[0];
        }
    }
}
