using System;
using System.Threading.Tasks;

namespace PersistenceCompatibilityTests
{
    public interface ITestPersistence
    {
        void Persist(Guid id, string originator);
        void Verify(Guid id, string originator);
    }
}