using System.Threading.Tasks;
using Common;
using NServiceBus;

namespace Version_6_2
{
    public static class SagaInitiator
    {
        public static void InitiateSaga(this IBus bus)
        {
            Parallel.ForEach(EndpointNames.FromEndPoint(TestRunner.EndpointName), endpoint =>
            {
                bus.SendLocal(new SagaInitiateRequestingMessage
                {
                    SourceEndpoint = endpoint.From,
                    TargetEndpoint = endpoint.To
                });
            });
        }
    }
}