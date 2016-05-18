using System;
using System.Collections.Generic;
using NServiceBus.Extensibility;
using NServiceBus.SagaPersisters.NHibernate;
using NServiceBus.Sagas;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Shared;

class TestPersistence : MarshalByRefObject, ITestPersistence
{
    private readonly SagaPersister persister;
    private readonly NHibernateSessionFactory factory;

    public TestPersistence()
    {
        factory = new NHibernateSessionFactory();
        persister = new SagaPersister();
    }

    public void Persist(Guid id, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
        {
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
        var session = factory.SessionFactory.Value.OpenSession();

        var data = persister.Get<TestSagaData>(id, new TestSessionProvider(session), new ContextBag()).GetAwaiter().GetResult();

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }

    public void Persist(Guid id, IList<int> data, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
        {
            persister.Save(new TestSagaDataWithList
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = originator,
                Ints = data
            }, new SagaCorrelationProperty("corr", id), new TestSessionProvider(session), new ContextBag())
                .GetAwaiter()
                .GetResult();

            session.Flush();
        }
    }

    public void Verify(Guid id, IList<int> ints, string originator)
    {
        var session = factory.SessionFactory.Value.OpenSession();
        var data = persister.Get<TestSagaDataWithList>(id, new TestSessionProvider(session), new ContextBag()).GetAwaiter().GetResult();

        Assert.AreEqual(id, data.Id);
        CollectionAssert.AreEqual(ints, data.Ints);
    }

    public void Persist(Guid id, string compositeValue, string originator)
    {
        using (var session = factory.SessionFactory.Value.OpenSession())
        {
            persister.Save(new TestSagaDataWithComposite
            {
                Id = id,
                OriginalMessageId = id.ToString(),
                Originator = originator,
                Composite = new TestSagaDataWithComposite.SagaComposite { Value = compositeValue }
            }, new SagaCorrelationProperty("corr", id), new TestSessionProvider(session), new ContextBag())
                .GetAwaiter()
                .GetResult();

            session.Flush();
        }
    }

    public void Verify(Guid id, string compositeValue, string originator)
    {
        var session = factory.SessionFactory.Value.OpenSession();

        var data = persister.Get<TestSagaDataWithComposite>(id, new TestSessionProvider(session), new ContextBag()).GetAwaiter().GetResult();

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
        Assert.AreEqual(compositeValue, data.Composite.Value);
    }
}