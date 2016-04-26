using System.Threading.Tasks;
using NServiceBus;

class Session : ISession
{
    readonly IBus instance;
    readonly Address localAddress;

    public Session(IBus instance)
    {
        this.instance = instance;
    }

    public Session(ISendOnlyBus instance)
    {
        this.instance = (IBus)instance;
        var unicastBus = (NServiceBus.Unicast.UnicastBus)instance;
        var machine = unicastBus.Configure.LocalAddress.Machine;
        var queue = unicastBus.Configure.LocalAddress.Queue;

        localAddress = new Address(queue, machine);
    }

    public async Task Send(object message)
    {
        await Task.Yield();
        instance.Send(message);
    }

    public async Task Publish(object message)
    {
        await Task.Yield();
        instance.Publish(message);
    }

    public async Task SendLocal(object message)
    {
        await Task.Yield();
        if (localAddress == null)
            instance.SendLocal(message);
        else
            instance.Send(localAddress, message);
    }

    public Task Close()
    {
        instance.Dispose();
        return Task.CompletedTask;
    }
}
