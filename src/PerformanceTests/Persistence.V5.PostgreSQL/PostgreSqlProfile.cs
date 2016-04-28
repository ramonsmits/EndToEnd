using NServiceBus;
using NServiceBus.PostgreSQL;

class RavenDBProfile : IProfile
{
    public void Configure(BusConfiguration cfg)
    {
        cfg.UsePersistence<PostgreSQLPersistence>();
    }
}
