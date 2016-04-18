using System.Threading.Tasks;
using NServiceBus;

class Session : ISession
{
    readonly IBus instance;

    public Session(IBus instance)
    {
        this.instance = instance;
    }

    public Task Send(object message)
    {
        instance.Send(message);
        return Task.FromResult(0);
    }

    public Task Publish(object message)
    {
        instance.Publish(message);
        return Task.FromResult(0);
    }

    public Task SendLocal(object message)
    {
        instance.SendLocal(message);
        return Task.FromResult(0);
    }
}
