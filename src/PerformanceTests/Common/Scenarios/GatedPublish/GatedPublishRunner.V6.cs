#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class GatedPublishRunner
{
    async Task Publish(object msg)
    {
        await EndpointInstance.Publish(msg);
    }
}

#endif
