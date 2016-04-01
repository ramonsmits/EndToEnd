namespace Common.Tests
{
    public class TestCaseFileName
    {
        public TestCaseFileName(string nsbVersion, string testCaseName, SerializationFormat format)
        {
            this.nsbVersion = nsbVersion;

            TestCaseName = testCaseName;
            Format = format;
        }

        string nsbVersion { get; }
        public string TestCaseName { get; }

        public SerializationFormat Format { get; }

        public override string ToString()
        {
            return $"NServiceBus {nsbVersion}_{TestCaseName}_.{Format}";
        }
    }
}