namespace Utils.Runner.AppDomains
{
    using System;

    public class TestDescriptor
    {
        public AppDomain AppDomain { get; set; }
        public string ProjectAssemblyPath { get; set; }
        public string PackageVersion { get; set; }
    }
}