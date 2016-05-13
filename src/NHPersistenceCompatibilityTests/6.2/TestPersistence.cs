using System;
using System.Threading.Tasks;
using NServiceBus.SagaPersisters.NHibernate;
using PersistenceCompatibilityTests;
using Version_6_2;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist(Guid id, string version)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        var persister = new SagaPersister(new TestSessionProvider(factory.SessionFactory.OpenSession()));

        persister.Save(new TestSagaData
        {
            Id = id,
            OriginalMessageId = id.ToString(),
            Originator = version
        });
    }

    public void Verify(Guid id, string version)
    {
        throw new System.NotImplementedException();
    }
}
