namespace Common.Runner.AppDomains
{
    using System;

    public class AppDomainRunner
    {
        public AppDomainRunner(AppDomainDescriptor appDomainDescriptor)
        {
            this.appDomainDescriptor = appDomainDescriptor;
        }

        public string PackageVersion => appDomainDescriptor.PackageVersion;

        public void Run(Action<ISerializationTester> action)
        {
            var appDomainEntry = (ISerializationTester)appDomainDescriptor.AppDomain.CreateInstanceFromAndUnwrap(appDomainDescriptor.ProjectAssemblyPath, "Tester");

            action(appDomainEntry);
        }

        readonly AppDomainDescriptor appDomainDescriptor;
    }
}