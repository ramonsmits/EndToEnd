using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using NServiceBus;
using NServiceBus.Logging;

class AzureServiceBusProfile : IProfile
{
    readonly string connectionstring = ProfileExtensionMethods.GetConnectionString(null, "AzureServiceBus");
    ILog Log = LogManager.GetLogger(nameof(AzureServiceBusProfile));

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .ConnectionString(connectionstring);

        CleanupTopology();
    }

    void CleanupTopology()
    {
        var manager = NamespaceManager.CreateFromConnectionString(connectionstring);
        var tasks = new List<Task>();
        tasks.AddRange(DeleteQueues(manager));
        tasks.AddRange(DeleteTopics(manager));
        Task.WhenAll(tasks);
        Log.Info("Cleanup completed!");
    }

    IEnumerable<Task> DeleteQueues(NamespaceManager manager)
    {
        foreach (var q in manager.GetQueues())
        {
            Log.InfoFormat("Deleting queue: {0}", q.Path);
            yield return manager.DeleteQueueAsync(q.Path);
        }
    }

    IEnumerable<Task> DeleteTopics(NamespaceManager manager)
    {
        foreach (var t in manager.GetTopics())
        {
            Log.InfoFormat("Deleting topic: {0}", t.Path);
            yield return manager.DeleteQueueAsync(t.Path);
        }
    }
}
