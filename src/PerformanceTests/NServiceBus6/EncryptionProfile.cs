using NServiceBus;

public class EncryptionProfile : IProfile
{
    public void Configure(EndpointConfiguration cfg)
    {
        cfg.RijndaelEncryptionService();
    }
}
