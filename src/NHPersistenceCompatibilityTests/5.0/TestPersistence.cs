using System;
using System.Collections.Generic;
using NHibernate;
using NServiceBus.SagaPersisters.NHibernate;
using NServiceBus.UnitOfWork;
using NServiceBus.UnitOfWork.NHibernate;
using NUnit.Framework;
using PersistenceCompatibilityTests;
using Version_5_0;


class TestPersistence : MarshalByRefObject, ITestPersistence
{
    public void Persist(Guid id, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();
        var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = factory.SessionFactory };
        var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

        ((IManageUnitsOfWork)unitOfWorkManager).Begin();

        persister.Save(new TestSagaData
        {
            Id = id,
            OriginalMessageId = id.ToString(),
            Originator = originator
        });

        ((IManageUnitsOfWork)unitOfWorkManager).End();
    }

    public void Verify(Guid id, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaData>();
        factory.Init();
        var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = factory.SessionFactory };
        var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

        var data = persister.Get<TestSagaData>(id);

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }

    public void Persist(Guid id, IList<int> data, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaDataWithList>();
        factory.Init();
        var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = factory.SessionFactory };
        var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

        ((IManageUnitsOfWork)unitOfWorkManager).Begin();

        persister.Save(new TestSagaDataWithList
        {
            Id = id,
            OriginalMessageId = id.ToString(),
            Originator = originator,
            Ints = data
        });

        ((IManageUnitsOfWork)unitOfWorkManager).End();
    }

    public void Verify(Guid id, IList<int> ints, string originator)
    {
        var factory = new NHibernateSessionFactory<TestSagaDataWithList>();
        factory.Init();
        var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = factory.SessionFactory };
        var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

        var data = persister.Get<TestSagaDataWithList>(id);

        Assert.AreEqual(id, data.Id);
        Assert.AreEqual(originator, data.Originator);
    }
}