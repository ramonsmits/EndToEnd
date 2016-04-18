using System.Threading.Tasks;

public interface ISession
{
    Task Send(object message);
    Task Publish(object message);
    Task SendLocal(object message);
}
