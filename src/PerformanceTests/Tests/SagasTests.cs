namespace Tests.Integration
{
    using NUnit.Framework;
    using Tests.Tools;

    [TestFixture]
    public class SagasTests
    {
        [TestCaseSource(typeof(EndpointGenerator), "Generate", Category = "Performance")]
        public void Concurrency0(TestInfo testInfo)
        {
            testInfo.Runner.Start("--saga",
                                  "--numberofmessages=2000",
                                  "--transport=msmq",
                                  "--serialization=json",
                                  "--persistence=inmemory",
                                  "--concurrency=10",
                                  "--numberOfThreads=1");
        }
    }
}