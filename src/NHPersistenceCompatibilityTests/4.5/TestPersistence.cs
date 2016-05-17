using System;
using NHibernate;
using NServiceBus.SagaPersisters.NHibernate;
using NServiceBus.UnitOfWork;
using NServiceBus.UnitOfWork.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_4_5;

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
        var unitOfWorkManager = new UnitOfWorkManager {SessionFactory = sessionFactory};
        var persister = new SagaPersister {UnitOfWorkManager = unitOfWorkManager};

        ((IManageUnitsOfWork) unitOfWorkManager).Begin();

        persister.Save(new TestSagaData
        {
            Id = id,
            OriginalMessageId = id.ToString(),
            Originator = originator
        });

        ((IManageUnitsOfWork) unitOfWorkManager).End();
    }

    public void Verify(Guid id, string originator)
    {
        var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = sessionFactory };
        var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

        var data = persister.Get<TestSagaData>(id);

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }
}