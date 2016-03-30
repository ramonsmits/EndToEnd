namespace Utils.Runner.AppDomains
{
    public class AppDomainRunner
    {
        readonly AppDomainDescriptor appDomainDescriptor;

        public string PackageVersion => appDomainDescriptor.PackageVersion;

        public AppDomainRunner(AppDomainDescriptor appDomainDescriptor)
        {
            this.appDomainDescriptor = appDomainDescriptor;
        }

        public void Start(params string[] args)
        {
            appDomainDescriptor.AppDomain.ExecuteAssembly(appDomainDescriptor.ProjectAssemblyPath, args);

            var stats = (Statistics)appDomainDescriptor.AppDomain.GetData("Statistics");

            stats.Dump();
        }
    }
}