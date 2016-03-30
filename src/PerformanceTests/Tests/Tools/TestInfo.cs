namespace Tests.Tools
{
    using Utils.Runner.AppDomains;
    using Utils.Runner.Nuget;

    public class TestInfo
    {
        public Package RunPackage { get; set; }
        public AppDomainRunner Runner { get; set; }

        public override string ToString()
        {
            return $"NServiceBus {RunPackage.Version}";
        }
    }
}