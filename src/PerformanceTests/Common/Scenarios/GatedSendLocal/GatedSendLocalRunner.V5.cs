#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedSendLocalRunner
{
    async Task SendLocal(Command msg)
    {
        EndpointInstance.SendLocal(msg);
    }
}
#endif
