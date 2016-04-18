#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class GatedSendLocalRunner
{
    async Task SendLocal(Command msg)
    {
        await this.EndpointInstance.SendLocal(msg);
    }
}

#endif
