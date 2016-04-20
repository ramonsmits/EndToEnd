﻿#if Version6
using NServiceBus;
using System.Threading.Tasks;

partial class SendRunner
{
    async Task SendLocal(Command msg)
    {
        await EndpointInstance.SendLocal(msg);
    }

    class Handler : IHandleMessages<Command>
    {
        public Task Handle(Command message, IMessageHandlerContext context)
        {
            Signal();
            return Task.FromResult(0);
        }
    }
}
#endif