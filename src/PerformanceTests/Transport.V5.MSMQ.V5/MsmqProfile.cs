
using System.Messaging;
using Common;
using NServiceBus;
using NServiceBus.Configuration.AdvanceExtensibility;
using NServiceBus.Unicast.Queuing;

class MsmqProfile : IProfile, ICreateTestData
{
    string endpointName = @".\private$\performancetests_nservicebus5.vshost.exe";

    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration.UseTransport<MsmqTransport>()
            .ConnectionString("deadLetter=false;journal=false");
    }

    public void CreateTestData(BusConfiguration configuration)
    {
        configuration.GetSettings().Get<IWantQueueCreated>();
        
        configuration.GetSettings().EndpointName();
    }

    public void CleanUpTestData(BusConfiguration configuration)
    {
        

        if (!MessageQueue.Exists(endpointName))
            return;

        var queue = new MessageQueue(endpointName);
        queue.Purge();
    }
}
