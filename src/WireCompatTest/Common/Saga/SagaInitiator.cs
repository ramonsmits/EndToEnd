using CommonMessages;
using NServiceBus;

public static class SagaInitiator
{
    public static void InitiateSaga(this IBus bus)
    {
        foreach (var endpoint in EndpointNames.All)
        {
            bus.SendLocal(new SagaInitiateRequestingMessage
                {
                    TargetEndpoint = endpoint
                });
        }
    }
}