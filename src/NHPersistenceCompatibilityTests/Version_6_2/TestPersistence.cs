using System;
using PersistenceCompatibilityTests;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist()
    {
        Console.WriteLine("Persist v6");
    }

    public void Verify()
    {
        throw new System.NotImplementedException();
    }
}