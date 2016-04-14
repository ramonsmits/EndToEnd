using NServiceBus;

partial class SagaInitiateRunner : BaseRunner
{
    protected override void Start()
    {
    }

    protected override void Stop()
    {
    }

    public class Command : ICommand
    {
        public Command(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

}
