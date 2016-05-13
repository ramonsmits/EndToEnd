using System;
using NHibernate;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Persistence;
using NServiceBus.SagaPersisters.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_7_0;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist(Guid id, string version)
    {
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

    public class TestSessionProvider : SynchronizedStorageSession, INHibernateSynchronizedStorageSession
    {
        public TestSessionProvider(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }

        public void ExecuteInTransaction(Action<ISession> operation)
        {
            operation(Session);
        }
    }

}