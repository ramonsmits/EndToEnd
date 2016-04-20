#if Version5
using System.Threading.Tasks;

partial class GatedPublishRunner
{
    async Task Publish(object message)
    {
        EndpointInstance.Publish(message);
    }
}
#endif
