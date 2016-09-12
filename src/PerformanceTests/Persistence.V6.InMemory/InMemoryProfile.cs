using NServiceBus;

class InMemoryProfile : IProfile
{
    public void Configure(EndpointConfiguration busConfiguration)
    {
        busConfiguration.UsePersistence<InMemoryPersistence>();
        BatchHelper.Instance = new BatchHelper.ParallelFor();
    }
}
