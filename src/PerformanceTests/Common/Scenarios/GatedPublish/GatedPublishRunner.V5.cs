#if Version5
using System.Threading.Tasks;
using NServiceBus;

partial class GatedPublishRunner
{
    async Task Publish(object message)
    {
        await Task.Run(() => EndpointInstance.Publish(message));
    }
}
#endif
