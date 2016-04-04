#if Version6
using Configuration = NServiceBus.EndpointConfiguration;
#else
using Configuration = NServiceBus.BusConfiguration;
#endif
using System;
using System.Collections.Generic;
using System.Linq;

static class ProfilesExtension
{
    public static void ApplyProfiles(this Configuration configuration)
    {
        foreach (var profile in GetProfiles())
        {
            var permutation = profile as IPermutation;
            if (permutation != null) throw new NotImplementedException("Get permutation instance here for injection.");
            profile.Configure(configuration);
        }
    }

    static IEnumerable<IProfile> GetProfiles()
    {
        var assemblies = AssemblyScanner.GetAssemblies();

        var type = typeof(IProfile);
        var types = assemblies
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));


        return types.Select(t => (IProfile)Activator.CreateInstance(t));
    }
}
