using System;
using System.Threading.Tasks;
using NHibernate;
using NServiceBus.SagaPersisters.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_6_2;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    private readonly ISessionFactory sessionFactory;

    public TestPersistence()
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        sessionFactory = factory.SessionFactory;

    }
    public void Persist(Guid id, string originator)
    {
        using (var session = sessionFactory.OpenSession())
        {
            var persister = new SagaPersister(new TestSessionProvider(session));

            persister.Save(new TestSagaData
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = originator
            });

            session.Flush();
        }
    }

    public void Verify(Guid id, string originator)
    {
        var session = sessionFactory.OpenSession();
        var persister = new SagaPersister(new TestSessionProvider(session));

        var data = persister.Get<TestSagaData>(id);

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }
}
