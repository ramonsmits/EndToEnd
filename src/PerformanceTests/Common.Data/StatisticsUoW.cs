
using System;
using System.Threading;
using System.Transactions;
using NServiceBus;
using NServiceBus.UnitOfWork;
using System.Diagnostics;
#if Version3
using NServiceBus.Config;
#endif

public class StatisticsUoW : IManageUnitsOfWork, INeedInitialization
{
#if Version6
    public System.Threading.Tasks.Task Begin()
    {
        DoBegin();

        return System.Threading.Tasks.Task.FromResult(0);
    }
#else
    public void Begin()
    {
        DoBegin();
    }
#endif

    void DoBegin()
    {
        if (!Statistics.Instance.First.HasValue)
        {
            Statistics.Instance.First = DateTime.Now;
        }

        if (Transaction.Current != null)
            Transaction.Current.TransactionCompleted += OnCompleted;
    }

    void OnCompleted(object sender, TransactionEventArgs e)
    {
        if (e.Transaction.TransactionInformation.Status != TransactionStatus.Committed)
        {
            return;
        }

        RecordSuccess();
    }

    void RecordSuccess()
    {
        Statistics.Instance.Last = DateTime.Now;
        Interlocked.Increment(ref Statistics.Instance.NumberOfMessages);
        Statistics.Instance.Signal();
        Trace.WriteLine("Message processed");
    }

#if Version6
    public System.Threading.Tasks.Task End(Exception ex = null)
    {
        DoEnd(ex);

        return System.Threading.Tasks.Task.FromResult(0);
    }
#else
    public void End(Exception ex = null)
    {
        DoEnd(ex);
    }
#endif

    private void DoEnd(Exception ex)
    {
        if (ex != null)
        {
            Interlocked.Increment(ref Statistics.Instance.NumberOfRetries);
            return;
        }

        if (Transaction.Current == null)
        {
            RecordSuccess();
        }
    }

#if Version5
    public void Customize(BusConfiguration configuration)
    {
        configuration.RegisterComponents(c => c.ConfigureComponent<StatisticsUoW>(DependencyLifecycle.InstancePerUnitOfWork));
    }
#endif

#if Version6
    public void Customize(EndpointConfiguration configuration)
    {
        configuration.RegisterComponents(c => c.ConfigureComponent<StatisticsUoW>(DependencyLifecycle.InstancePerUnitOfWork));
    }
#endif

    public void Init()
    {
#if Version3
        Configure.Instance.Configurer.ConfigureComponent<StatisticsUoW>(DependencyLifecycle.InstancePerUnitOfWork);
#elif Version4
        Configure.Component<StatisticsUoW>(DependencyLifecycle.InstancePerUnitOfWork);
#endif
    }
}