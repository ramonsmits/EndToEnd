namespace Utils.Runner.AppDomains
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    public class CustomAssemblyLoader : MarshalByRefObject
    {
        private AppDomain appDomain;

        public CustomAssemblyLoader(AppDomain appDomain)
        {
            this.appDomain = appDomain;
        }

        public void AddAssemblyRedirect(string shortName, Version targetVersion, byte[] publicKeyToken)
        {
            var resolver = new BindingRedirectResolver(shortName, targetVersion, publicKeyToken);

            appDomain.AssemblyResolve += resolver.Resolve;
        }

        [Serializable]
        private class BindingRedirectResolver
        {
            private byte[] publicKeyToken;
            private string shortName;
            private Version targetVersion;

            public BindingRedirectResolver(string shortName, Version targetVersion, byte[] publicKeyToken)
            {
                this.shortName = shortName;
                this.targetVersion = targetVersion;
                this.publicKeyToken = publicKeyToken;
            }

            public Assembly Resolve(object sender, ResolveEventArgs args)
            {
                var requestedAssembly = new AssemblyName(args.Name);
                if (requestedAssembly.Name != shortName)
                    return null;

                Debug.WriteLine("Redirecting assembly load of " + args.Name
                                + ",\tloaded by " + (args.RequestingAssembly?.FullName ?? "(unknown)"));

                requestedAssembly.Version = targetVersion;
                requestedAssembly.SetPublicKeyToken(publicKeyToken);
                requestedAssembly.CultureInfo = CultureInfo.InvariantCulture;

                return Assembly.Load(requestedAssembly);
            }
        }
    }
}