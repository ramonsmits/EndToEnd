using System;
using System.Collections.Generic;
using NServiceBus.SagaPersisters.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_6_2;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    private readonly NHibernateSessionFactory factory;

    public TestPersistence()
    {
        factory = new NHibernateSessionFactory();
    }

    public void Persist(Guid id, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
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
        using (var session = factory.SessionFactory.Value.OpenSession())
        {
            var persister = new SagaPersister(new TestSessionProvider(session));

            var data = persister.Get<TestSagaData>(id);

            Assert.AreEqual(id, data.Id);
            Assert.AreEqual(originator, data.Originator);
        }
    }

    public void Persist(Guid id, IList<int> data, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
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
        var session = factory.SessionFactory.Value.OpenSession();
        var persister = new SagaPersister(new TestSessionProvider(session));

        var data = persister.Get<TestSagaDataWithList>(id);

        Assert.AreEqual(id, data.Id);
        CollectionAssert.AreEqual(ints, data.Ints);
    }

    public void Persist(Guid id, string compositeValue, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
        {
            var persister = new SagaPersister(new TestSessionProvider(session));

            persister.Save(new TestSagaDataWithComposite
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = originator,
                Composite = new TestSagaDataWithComposite.SagaComposite { Value = compositeValue }
            });

            session.Flush();
        }
    }

    public void Verify(Guid id, string compositeValue, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
        {
            var persister = new SagaPersister(new TestSessionProvider(session));

            var data = persister.Get<TestSagaDataWithComposite>(id);

            Assert.AreEqual(id, data.Id);
            Assert.AreEqual(originator, data.Originator);
            Assert.AreEqual(compositeValue, data.Composite.Value);
        }
    }
}
