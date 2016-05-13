using System;
using System.Threading.Tasks;
using NServiceBus.SagaPersisters.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_6_2;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist(Guid id, string version)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        using (var session = factory.SessionFactory.OpenSession())
        {
            var persister = new SagaPersister(new TestSessionProvider(session));

            persister.Save(new TestSagaData
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = version
            });

            session.Flush();
        }
    }

    public void Verify(Guid id, string version)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        var session = factory.SessionFactory.OpenSession();
        var persister = new SagaPersister(new TestSessionProvider(session));

        var data = persister.Get<TestSagaData>(id);

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(version, data.Originator);
    }
}
