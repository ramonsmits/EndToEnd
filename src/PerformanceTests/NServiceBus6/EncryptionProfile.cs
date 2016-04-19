using NServiceBus;
using Tests.Permutations;
using Variables;

public class EncryptionProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { get; set; }

    public void Configure(EndpointConfiguration cfg)
    {
        if (Permutation.Encryption == Encryption.Rijndael)
        {
            cfg.RijndaelEncryptionService();
        }
    }
}
