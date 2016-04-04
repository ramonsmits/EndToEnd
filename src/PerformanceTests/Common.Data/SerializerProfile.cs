#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif

using Categories;
using NServiceBus;
using Variables;

public class SerializerProfile : IProfile, IPermutation
{
    public Permutation Permutation { get; set; }

    public void Configure(Configuration cfg)
    {
        switch (Permutation.Serializer)
        {
            case Serialization.Xml:
                cfg.UseSerialization<XmlSerializer>();
                break;
            case Serialization.Json:
                cfg.UseSerialization<JsonSerializer>();
                break;
        }
    }
}
