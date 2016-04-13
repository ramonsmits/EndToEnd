using System;

using NServiceBus;
using Tests.Permutations;
using Variables;

partial class SagaInitiateRunner : BaseRunner, IThrowIfPermutationIsNotAllowed
{
    public void ThrowIfPermutationIsNotAllowed(Permutation permutation)
    {
        if (permutation.Persister == Persistence.InMemory)
            throw new InvalidOperationException("This test has no use running with InMemory persistence.");
    }

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
