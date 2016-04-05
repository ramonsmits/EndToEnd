using NServiceBus;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;
using Categories;

class ConcurrencyProfile : IProfile, INeedPermutation, IProvideConfiguration<TransportConfig>
{
    static int MaximumConcurrencyLevel;
    public Permutation Permutation { private get; set; }


    public void Configure(BusConfiguration cfg)
    {
        MaximumConcurrencyLevel = ConcurrentyLevelConverter.Convert(Permutation.ConcurrencyLevel);
    }

    public TransportConfig GetConfiguration()
    {
        return new TransportConfig
        {
            MaximumConcurrencyLevel = MaximumConcurrencyLevel,
        };
    }
}
