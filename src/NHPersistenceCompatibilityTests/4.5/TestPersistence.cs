using System;
using PersistenceCompatibilityTests;

namespace _4._5
{
    public class TestPersistence : MarshalByRefObject, ITestPersistence
    {
        public void Persist(Guid id, string originator)
        {
            throw new NotImplementedException();
        }

        public void Verify(Guid id, string originator)
        {
            throw new NotImplementedException();
        }
    }
}