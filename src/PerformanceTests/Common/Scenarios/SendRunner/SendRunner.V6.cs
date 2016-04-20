#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class SendRunner
{
    async Task SendLocal(Command msg)
    {
        await this.EndpointInstance.SendLocal(msg);
    }
}
#endif