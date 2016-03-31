namespace Common.Tests
{
    using System.Collections.Generic;

    public interface ITestCaseFinder
    {
        List<TestCase> Find(string appDomainName);
    }
}