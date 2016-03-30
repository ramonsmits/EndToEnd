using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.ObjectBuilder;
using NServiceBus.Serializers.Json;

class JsonSerializerFacade : ISerializerFacade
{
    public JsonSerializerFacade(params Type[] objectTypes)
    {
        mapper = new MessageMapper();
        serializer = new JsonMessageSerializer(mapper);

        SetupTypeHeader(objectTypes);
    }

    public void Serialize(Stream stream, object instance)
    {
        serializer.Serialize(new[]
        {
            instance
        }, stream);
    }

    public object[] Deserialize(Stream stream)
    {
        return serializer.Deserialize(stream);
    }

    public object CreateInstance(Type type)
    {
        return type.IsInterface ? mapper.CreateInstance(type) : Activator.CreateInstance(type);
    }

    void SetupTypeHeader(Type[] objectTypes)
    {
        Debug.WriteLine("xxx");
        AppDomain.CurrentDomain.GetAssemblies().Where(a => a.IsDynamic == false).ToList().ForEach(a => Debug.WriteLine(a.CodeBase));

        Configure.With();
        Configure.Instance.Builder = new FakeBuilder
        {
            Bus = new FakeBus
            {
                CurrentMessageContext = new FakeMessageContext
                {
                    Headers = headers
                }
            }
        };

        headers[typeHeaderName] = string.Join(";", objectTypes.Select(ot => ot.AssemblyQualifiedName).ToArray());

        mapper.Initialize(objectTypes);
    }

    MessageMapper mapper;
    JsonMessageSerializer serializer;

    Dictionary<string, string> headers = new Dictionary<string, string>();
    string typeHeaderName = "NServiceBus.EnclosedMessageTypes";

    class FakeBuilder : IBuilder
    {
        public IBus Bus { get; set; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object Build(Type typeToBuild)
        {
            throw new NotImplementedException();
        }

        public IBuilder CreateChildBuilder()
        {
            throw new NotImplementedException();
        }

        public T Build<T>()
        {
            return (T)Bus;
        }

        public IEnumerable<T> BuildAll<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> BuildAll(Type typeToBuild)
        {
            throw new NotImplementedException();
        }

        public void BuildAndDispatch(Type typeToBuild, Action<object> action)
        {
            throw new NotImplementedException();
        }
    }

    class FakeBus : IBus
    {
        public T CreateInstance<T>()
        {
            throw new NotImplementedException();
        }

        public T CreateInstance<T>(Action<T> action)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(params T[] messages)
        {
            throw new NotImplementedException();
        }

        public void Publish<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>()
        {
            throw new NotImplementedException();
        }

        public void Subscribe(Type messageType, Predicate<object> condition)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T>(Predicate<T> condition)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe(Type messageType)
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T>()
        {
            throw new NotImplementedException();
        }

        public ICallback SendLocal(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback SendLocal<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(string destination, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Address address, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(string destination, string correlationId, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send(Address address, string correlationId, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(string destination, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback Send<T>(Address address, string correlationId, Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public ICallback SendToSites(IEnumerable<string> siteKeys, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(TimeSpan delay, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public ICallback Defer(DateTime processAt, params object[] messages)
        {
            throw new NotImplementedException();
        }

        public void Reply(params object[] messages)
        {
            throw new NotImplementedException();
        }

        public void Reply<T>(Action<T> messageConstructor)
        {
            throw new NotImplementedException();
        }

        public void Return<T>(T errorEnum)
        {
            throw new NotImplementedException();
        }

        public void HandleCurrentMessageLater()
        {
            throw new NotImplementedException();
        }

        public void ForwardCurrentMessageTo(string destination)
        {
            throw new NotImplementedException();
        }

        public void DoNotContinueDispatchingCurrentMessageToHandlers()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> OutgoingHeaders { get; }
        public IMessageContext CurrentMessageContext { get; set; }
    }

    class FakeMessageContext : IMessageContext
    {
        public string Id { get; }
        public string ReturnAddress { get; }
        public Address ReplyToAddress { get; }
        public DateTime TimeSent { get; }
        public IDictionary<string, string> Headers { get; set; }
    }
}