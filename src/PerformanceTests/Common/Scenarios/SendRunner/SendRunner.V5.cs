#if Version5
using System.Threading.Tasks;

partial class SendRunner
{
    async Task SendLocal(Command msg)
    {
        EndpointInstance.SendLocal(msg);
    }
}
#endif