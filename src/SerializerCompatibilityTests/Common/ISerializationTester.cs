namespace Common
{
    using System;
    using Tests;

    public interface ISerializationTester
    {
        //HINT: we are passing type of test case to make sure that during the test
        //      testcase instance is create in app domain spawn for each nsb version.
        //      This is important as NSB dynamically creates types for interfaces.
        void Serialize(Type testCase, SerializationFormat format, string filePath);
        void Verify(Type testCase, SerializationFormat format, string filePath);
    }
}