using NServiceBus;

public class EncryptionProfile : IProfile
{
    public void Configure(BusConfiguration cfg)
    {
        cfg.RijndaelEncryptionService();
    }
}
