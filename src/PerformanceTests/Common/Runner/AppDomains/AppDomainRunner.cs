namespace Utils.Runner.AppDomains
{
    public class AppDomainRunner
    {
        readonly TestDescriptor testDescriptor;

        public string PackageVersion => testDescriptor.PackageVersion;

        public AppDomainRunner(TestDescriptor testDescriptor)
        {
            this.testDescriptor = testDescriptor;
        }

        public void Start(params string[] args)
        {
            testDescriptor.AppDomain.ExecuteAssembly(testDescriptor.ProjectAssemblyPath, args);

            var stats = (Statistics)testDescriptor.AppDomain.GetData("Statistics");

            stats.Dump();
        }
    }
}