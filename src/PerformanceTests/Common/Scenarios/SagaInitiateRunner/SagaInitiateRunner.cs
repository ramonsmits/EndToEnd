using System;
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
            Identifier = id;
        }

        public int Identifier { get; set; }
    }

}
