using NServiceBus.Config;
using NServiceBus.Config.ConfigurationSource;

class ProvideConfiguration : IProvideConfiguration<MessageForwardingInCaseOfFaultConfig>
{
    public MessageForwardingInCaseOfFaultConfig GetConfiguration()
    {
        return new MessageForwardingInCaseOfFaultConfig
        {
            ErrorQueue = "testharness.error"
        };
    }
}