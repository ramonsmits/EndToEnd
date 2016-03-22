using System;
using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

public class ProvideTransportConfiguration : IProvideConfiguration<TransportConfig>
{

    public TransportConfig GetConfiguration()
    {
        NServiceBus.Logging.LogManager.GetLogger(typeof(ProvideTransportConfiguration)).InfoFormat("Set MaximumConcurrencyLevel to environment processor count: {0}", Environment.ProcessorCount);
        return new TransportConfig
        {
            MaximumConcurrencyLevel = Environment.ProcessorCount,
            MaxRetries = 0,
        };
    }
}