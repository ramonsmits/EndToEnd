using System;
using PersistenceCompatibilityTests;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist()
    {
        Console.WriteLine("Persis v7");
    }

    public void Verify()
    {
        throw new System.NotImplementedException();
    }
}