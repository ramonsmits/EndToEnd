using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NServiceBus;
using NServiceBus.Azure.Transports.WindowsAzureStorageQueues.Config;
using NServiceBus.Configuration.AdvanceExtensibility;

class AzureStorageQueuesProfile : IProfile, ICreateTestData
{
    EndpointConfiguration endpointConfiguration;

    public void Configure(EndpointConfiguration endpointConfiguration)
    {
        this.endpointConfiguration = this.endpointConfiguration;
        endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
    }

    public void CreateTestData(EndpointConfiguration configuration)
    {
        throw new NotImplementedException();
    }

    public void CleanUpTestData(EndpointConfiguration configuration)
    {
        var connectionString = endpointConfiguration.GetSettings().GetOrDefault<string>(WellKnownConfigurationKeys.ReceiverConnectionString);

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var queueClient = storageAccount.CreateCloudQueueClient();
        IEnumerable<CloudQueue> allQueues = queueClient.ListQueues().ToArray();

        var countdown = new CountdownEvent(allQueues.Count());

        foreach (var queue in allQueues)
        {
            queue.ClearAsync().ContinueWith(t => countdown.Signal());
        }

        if (countdown.Wait(TimeSpan.FromMinutes(1)) == false)
        {
            throw new TimeoutException("Waiting for cleaning queues took too much.");
        }
    }
}
