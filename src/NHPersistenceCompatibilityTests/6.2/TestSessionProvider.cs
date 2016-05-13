using System;
using NHibernate;
using NServiceBus.Persistence.NHibernate;

namespace Version_6_2
{
    public class TestSessionProvider : IStorageSessionProvider
    {
        public TestSessionProvider(ISession session)
        {
            Session = session;
        }

        public ISession Session { get; }

        public void ExecuteInTransaction(Action<ISession> operation)
        {
            operation(Session);
        }
    }
}