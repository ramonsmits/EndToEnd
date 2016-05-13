namespace PersistenceCompatibilityTests
{
    public interface ITestPersistence
    {
        void Persist();
        void Verify();
    }
}