﻿using System;
using NServiceBus.Saga;
using NServiceBus.SagaPersisters.NHibernate;
using NServiceBus.UnitOfWork;
using NServiceBus.UnitOfWork.NHibernate;
using Version_4_5;

public class Persister
{
    public void Save<T>(T data, string correlationPropertyName = null, string correlationPropertyValue = null) where T : IContainSagaData
    {
        using (var sessionFactory = NHibernateSessionFactory.Create())
        {
            var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = sessionFactory };
            var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

            ((IManageUnitsOfWork)unitOfWorkManager).Begin();

            persister.Save(data);

            ((IManageUnitsOfWork)unitOfWorkManager).End();
        }
    }

    public void Update<T>(T data) where T : IContainSagaData
    {
        using (var sessionFactory = NHibernateSessionFactory.Create())
        {
            var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = sessionFactory};
            var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

            ((IManageUnitsOfWork)unitOfWorkManager).Begin();

            persister.Update(data);

            ((IManageUnitsOfWork)unitOfWorkManager).End();
        }
    }

    public T Get<T>(Guid id) where T : IContainSagaData
    {
        using (var sessionFactory = NHibernateSessionFactory.Create())
        {
            var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = sessionFactory };
            var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

            ((IManageUnitsOfWork)unitOfWorkManager).Begin();

            var data = persister.Get<T>(id);

            //Make sure all lazy properties are fetched before returning result
            ObjectFetcher.Traverse(data, typeof(T));

            ((IManageUnitsOfWork)unitOfWorkManager).End();

            return data;
        }
    }

    public void Complete<T>(T data) where T : IContainSagaData
    {
        using (var sessionFactory = NHibernateSessionFactory.Create())
        {
            var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = sessionFactory };
            var persister = new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

            ((IManageUnitsOfWork)unitOfWorkManager).Begin();

            persister.Complete(data);

            ((IManageUnitsOfWork)unitOfWorkManager).End();
        }
    }

    public T GetByCorrelationProperty<T>(string correlationPropertyName, object correlationPropertyValue) where T : IContainSagaData
    {
        using (var sessionFactory = NHibernateSessionFactory.Create())
        {
            var unitOfWorkManager = new UnitOfWorkManager { SessionFactory = sessionFactory };
            var persister = (ISagaPersister)new SagaPersister { UnitOfWorkManager = unitOfWorkManager };

            ((IManageUnitsOfWork)unitOfWorkManager).Begin();

            var data = persister.Get<T>(correlationPropertyName, correlationPropertyValue);

            //Make sure all lazy properties are fetched before returning result
            ObjectFetcher.Traverse(data, typeof(T));

            ((IManageUnitsOfWork)unitOfWorkManager).End();

            return data;
        }
    }
}