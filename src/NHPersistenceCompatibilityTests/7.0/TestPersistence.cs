using System;
using NHibernate;
using NServiceBus.Extensibility;
using NServiceBus.SagaPersisters.NHibernate;
using NServiceBus.Sagas;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_7_0;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    private readonly SagaPersister persister;
    private readonly ISessionFactory sessionFactory;

    public TestPersistence()
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        sessionFactory = factory.SessionFactory;
        persister = new SagaPersister();
    }
    public void Persist(Guid id, string originator)
    {
        using (var session = sessionFactory.OpenSession())
        {
            var persister = new SagaPersister();

            persister.Save(new TestSagaData
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = originator
            }, new SagaCorrelationProperty("corr", id), new TestSessionProvider(session), new ContextBag())
                .GetAwaiter()
                .GetResult();

            session.Flush();
        }
    }

    public void Verify(Guid id, string originator)
    {
        var session = sessionFactory.OpenSession();

        var data = persister.Get<TestSagaData>(id, new TestSessionProvider(session), new ContextBag()).GetAwaiter().GetResult();

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }
}