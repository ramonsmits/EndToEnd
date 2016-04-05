using Categories;
using NServiceBus;

class ConcurrencyProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { private get; set; }


    public void Configure(EndpointConfiguration cfg)
    {
        cfg.LimitMessageProcessingConcurrencyTo(ConcurrentyLevelConverter.Convert(Permutation.ConcurrencyLevel));
    }
}
