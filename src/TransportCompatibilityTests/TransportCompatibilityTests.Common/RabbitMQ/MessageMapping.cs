namespace TransportCompatibilityTests.Common.RabbitMQ
{
    using System;

    [Serializable]
    public class MessageMapping
    {
        public Type MessageType { get; set; }
        public string TransportAddress { get; set; }
    }
}
