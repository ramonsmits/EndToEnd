namespace Tests.CompatibilityTests
{
    using System;
    using Common.Runner.AppDomains;
    using Common.Runner.Nuget;
    using Common.Tests;

    public class TestInfo
    {
        public enum TestType
        {
            Serialization,
            Deserialization
        }

        public Package RunPackage { get; set; }
        public string CheckVersion { get; set; }
        public Type TestCase { get; set; }
        public TestType Type { get; set; }
        public SerializationFormat Format { get; set; }
        public AppDomainRunner Runner { get; set; }

        public override string ToString()
        {
            if (Type == TestType.Serialization)
            {
                return string.Format("Serialization_{0}_{1}_{2}", TestCase.Name, Format, RunPackage.Version);
            }
            return string.Format("Deserialization_{0}_{1}_{2}->{3}", TestCase.Name, Format, CheckVersion, RunPackage.Version);
        }
    }
}