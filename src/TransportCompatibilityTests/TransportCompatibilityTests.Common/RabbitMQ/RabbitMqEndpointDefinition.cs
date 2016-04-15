namespace TransportCompatibilityTests.Common.RabbitMQ
{
    using System;

    [Serializable]
    public class RabbitMqEndpointDefinition : EndpointDefinition
    {
        public override string TransportName => "RabbitMQ";
        public MessageMapping[] Mappings { get; set; }

        public RabbitMqEndpointDefinition()
        {
            Mappings = new MessageMapping[0];
        }
    }
}
