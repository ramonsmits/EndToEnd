#if !Version3
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

class TransportConfigOverride : IProvideConfiguration<TransportConfig>
{
    public static int MaximumConcurrencyLevel;
    public TransportConfig GetConfiguration()
    {
        return new TransportConfig
        {
#if !Version6
            MaximumConcurrencyLevel = MaximumConcurrencyLevel,
#endif
            MaxRetries = 10
        };
    }
}
#endif