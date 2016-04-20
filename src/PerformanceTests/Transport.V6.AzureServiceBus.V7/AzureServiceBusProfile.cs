using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using NServiceBus;
using NServiceBus.AzureServiceBus;
using NServiceBus.Logging;

class AzureServiceBusProfile : IProfile, ISetup
{
    readonly string connectionstring = ProfileExtensionMethods.GetConnectionString(null, "AzureServiceBus");
    ILog Log = LogManager.GetLogger(nameof(AzureServiceBusProfile));

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        endpointConfiguration
            .UseTransport<AzureServiceBusTransport>()
            .UseTopology<ForwardingTopology>()
            .ConnectionString(connectionstring);
    }

    public void Setup()
    {
        CleanupTopology();
    }

    void CleanupTopology()
    {
        var manager = NamespaceManager.CreateFromConnectionString(connectionstring);

        var tasks = new List<Task>();
        tasks.AddRange(DeleteQueues(manager));
        tasks.AddRange(DeleteTopics(manager));

        Log.Info("Waiting for delete tasks to finish...");
        Task.WhenAll(tasks);

        var topicCount = 0;
        var queueCount = 0;

        while (0 < (topicCount = manager.GetTopics().Count()) || 0 < (queueCount = manager.GetQueues().Count()))
        {
            Thread.Sleep(100);
            Log.InfoFormat("Waiting until all topics ({0}) and queues ({1}) are deleted...", topicCount, queueCount);
        }

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
