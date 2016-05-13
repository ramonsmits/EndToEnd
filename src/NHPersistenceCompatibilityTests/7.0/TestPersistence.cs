using System;
using NServiceBus.Extensibility;
using NServiceBus.SagaPersisters.NHibernate;
using NServiceBus.Sagas;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_7_0;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist(Guid id, string version)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        using (var session = factory.SessionFactory.OpenSession())
        {
            var persister = new SagaPersister();
            
                persister.Save(new TestSagaData
                {
                    Id = id,
                    OriginalMessageId = id.ToString(),
                    Originator = version
                }, new SagaCorrelationProperty("corr", id), new TestSessionProvider(session), new ContextBag())
                    .GetAwaiter()
                    .GetResult();
                
            session.Flush();
        }
    }

    public void Verify(Guid id, string version)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();

        var session = factory.SessionFactory.OpenSession();
        var persister = new SagaPersister();

        var data = persister.Get<TestSagaData>(id, new TestSessionProvider(session), new ContextBag()).GetAwaiter().GetResult();

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(version, data.Originator);
    }
}