using System;
using System.Collections.Concurrent;
using System.Linq;

namespace TransportCompatibilityTests.Common
{
    public class MessageStore
    {
        private readonly ConcurrentDictionary<Guid, Type> messageIds = new ConcurrentDictionary<Guid, Type>();

        public void Add<T>(Guid id)
        {
            messageIds.AddOrUpdate(id, typeof(T), (i, v) => typeof(T));
        }

        public Guid[] Get<T>()
        {
            return messageIds.ToArray()
                              .Where(kv => kv.Value == typeof (T)).Select(kv => kv.Key).ToArray();
        }

        public Guid[] GetAll()
        {
            return messageIds.ToArray()
                              .Select(kv => kv.Key).ToArray();
        }
    }
}