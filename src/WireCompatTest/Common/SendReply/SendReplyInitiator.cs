using CommonMessages;
using NServiceBus;

public static class SendReplyInitiator
{
    public static void InitiateSendReply(this IBus bus)
    {
        foreach (var endpoint in EndpointNames.All)
        {
            bus.Send(endpoint, new SendReplyFirstMessage
                {
                    Sender = TestRunner.EndpointName,
                    EncryptedProperty = "Secret"
                });
        }
    }
}