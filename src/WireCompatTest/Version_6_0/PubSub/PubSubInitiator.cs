using System;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus;

public static class PubSubInitiator
{
    public static async Task InitiatePubSub(this IEndpointInstance bus)
    {
        var messagesAssemblyName = Assembly.GetExecutingAssembly().GetName().Name + ".Messages";
        var typeName = messagesAssemblyName + ".MyEvent, " + messagesAssemblyName;
        var messageType = Type.GetType(typeName, true);
        var message = (dynamic)Activator.CreateInstance(messageType);
        message.Sender = TestRunner.EndpointName;
        await bus.Publish((object)message).ConfigureAwait(false);
    }
}