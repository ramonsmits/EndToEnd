using System;
using NServiceBus;

partial class SagaInitiateRunner : BaseRunner
{
    public class Command : ICommand
    {
        public Command(int id)
        {
            Identifier = id;
        }

        public int Identifier { get; set; }
    }

}
