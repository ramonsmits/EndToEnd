using NServiceBus;
using Tests.Permutations;

class ConcurrencyProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { private get; set; }

    public void Configure(EndpointConfiguration cfg)
    {
        cfg.LimitMessageProcessingConcurrencyTo(1);//ConcurrencyLevelConverter.Convert(Permutation.ConcurrencyLevel));
    }
}
