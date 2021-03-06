#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
using System.Collections.Generic;

static class ProfilesExtension
{
    public static void ApplyProfiles(this Configuration configuration, IContext ctx)
    {
        var log = NLog.LogManager.GetLogger("Profiles");
        foreach (var profile in GetProfiles())
        {
            log.Info("Applying profile: {0}", profile);

            var injectPermutation = profile as INeedPermutation;
            if (injectPermutation != null) injectPermutation.Permutation = ctx.Permutation;

            var injectContext = profile as INeedContext;
            if (injectContext != null) injectContext.Context = ctx;

            profile.Configure(configuration);
        }
    }

    static IEnumerable<IProfile> GetProfiles()
    {
        return AssemblyScanner.GetAll<IProfile>();
    }
}