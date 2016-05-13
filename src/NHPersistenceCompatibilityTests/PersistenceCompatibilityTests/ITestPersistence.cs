using System;
using System.Threading.Tasks;

namespace PersistenceCompatibilityTests
{
    public interface ITestPersistence
    {
        void Persist(Guid id, string version);
        void Verify(Guid id, string version);
    }
}