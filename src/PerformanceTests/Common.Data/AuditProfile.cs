#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
namespace NServiceBus5
{
    using Categories;
    using NServiceBus;
    using NServiceBus.Features;

    class AuditProfile : IProfile, INeedPermutation
    {
        public Permutation Permutation { private get; set; }

        public void Configure(Configuration cfg)
        {
            if (Permutation.AuditMode == Variables.Audit.Off) cfg.DisableFeature<Audit>();
        }
    }
}