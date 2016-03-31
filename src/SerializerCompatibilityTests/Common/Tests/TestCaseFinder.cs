namespace Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    public class TestCaseFinder
    {
        public List<TestCase> FindAll()
        {
            var testCaseTypes = typeof(TestCase).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(TestCase)) && !Attribute.IsDefined(type, typeof(IgnoreAttribute)));

            var allTestCases = testCaseTypes.Select(type => (TestCase)Activator.CreateInstance(type));

            return allTestCases.ToList();
        }
    }
}