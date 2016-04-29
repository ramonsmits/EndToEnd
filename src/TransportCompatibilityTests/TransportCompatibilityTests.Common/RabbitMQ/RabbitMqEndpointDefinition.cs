namespace TransportCompatibilityTests.Common.RabbitMQ
{
    using System;

    [Serializable]
    public class RabbitMqEndpointDefinition : EndpointDefinition
    {
        public override string TransportName => "RabbitMQ";
        public MessageMapping[] Mappings { get; set; }
        public Topology RoutingTopology { get; set; }

        public RabbitMqEndpointDefinition()
        {
            RoutingTopology = Topology.Convention;
            Mappings = new MessageMapping[0];
        }
    }

    public enum Topology
    {
        Direct,
        Convention
    }
}
