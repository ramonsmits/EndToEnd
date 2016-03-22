using Raven.Client.Document;

class DoSomethingToAccessDataFromRavenDb : ISimulateUserDataAccess
{
    public void DoSomething()
    {
        using (var store = new DocumentStore
        {
            ConnectionStringName = "NServiceBus/Persistence/RavenDB",
            DefaultDatabase = "TestHarness.IncomingMessages"
        })
        {
            store.Initialize();

            using (var session = store.OpenSession())
            {
                session.Advanced.DocumentStore.DatabaseCommands.GetIndex("TimeoutsIndex");
            }
        }
    }
}
