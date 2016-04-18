using System.Threading.Tasks;
using NLog.Fluent;
using NServiceBus;
using NServiceBus.Config;

/// <summary>
/// Performs a continious test where a batch of messages is send via the bus without
/// a transaction and a handler processes these in parallel. Once all messages are
/// received it repeats this. Due to the fact that the sending is not transactional
/// the handler will already process messages while the batch is still being send.
/// </summary>
partial class GatedPublishRunner : LoopRunner, IConfigureUnicastBus
{
    protected override int BatchSize { get; set; } = 16;
    protected override Task SendMessage()
    {
        return Publish(new Event());
    }

   public class Event : IEvent
    {
    }

    public MessageEndpointMappingCollection GenerateMappings()
    {
        var mappings = new MessageEndpointMappingCollection();

        var messageType = typeof(Event);

        Log.Info($"Mapping {messageType} to {EndpointName}");

        mappings.Add(new MessageEndpointMapping
        {
            AssemblyName = messageType.Assembly.FullName,
            TypeFullName = messageType.FullName,
            Endpoint = EndpointName
        });

        return mappings;
    }
}


