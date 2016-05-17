using System;
using System.Collections.Generic;
using NHibernate;
using NServiceBus.SagaPersisters.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_6_2;

class TestPersistence : MarshalByRefObject, ITestPersistence
{

    public void Persist(Guid id, string originator)
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
                Originator = originator
            });

            session.Flush();
        }
    }

    public void Verify(Guid id, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();
        var session = factory.SessionFactory.OpenSession();
        var persister = new SagaPersister(new TestSessionProvider(session));

        var data = persister.Get<TestSagaData>(id);

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }

    public void Persist(Guid id, IList<int> data, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaDataWithList>();
        factory.Init();
        using (var session = factory.SessionFactory.OpenSession())
        {
            var persister = new SagaPersister(new TestSessionProvider(session));

            persister.Save(new TestSagaDataWithList
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = originator,
                Ints = data
            });

            session.Flush();
        }
    }
    
    public void Verify(Guid id, IList<int> ints, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaDataWithList>();
        factory.Init();
        var session = factory.SessionFactory.OpenSession();
        var persister = new SagaPersister(new TestSessionProvider(session));

        var data = persister.Get<TestSagaDataWithList>(id);

        Assert.AreEqual(id, data.Id);
        CollectionAssert.AreEqual(ints, data.Ints);
    }
}
