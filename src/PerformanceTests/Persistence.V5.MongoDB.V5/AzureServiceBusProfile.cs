using NServiceBus;
using NServiceBus.Persistence.MongoDB;

class MongoDBProfile : IProfile
{
    public void Configure(BusConfiguration busConfiguration)
    {
        busConfiguration
            .UsePersistence<MongoDbPersistence>()
            .SetConnectionStringName("MongoDB");
    }
}
