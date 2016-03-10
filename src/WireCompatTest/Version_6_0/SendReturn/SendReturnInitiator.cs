using System.Threading.Tasks;
using CommonMessages;
using NServiceBus;

public static class SendReturnInitiator
{

    public static async Task InitiateSendReturn(this IEndpointInstance bus)
    {
        foreach (var endpointName in EndpointNames.All)
        {
            var sendOptions = new SendOptions();
            sendOptions.SetDestination(endpointName);

            var result = await bus.Request<int>(new SendReturnMessage(), sendOptions);
            Asserter.IsTrue(5 == result, "Incorrect property value");
            SendReturnVerifier.ReplyReceivedFrom.Add(endpointName);
        }
    }
}