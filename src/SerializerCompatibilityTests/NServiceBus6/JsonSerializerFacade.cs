using System;
using System.IO;
using Common;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Serialization;
using NServiceBus.Settings;

class JsonSerializerFacade : ISerializerFacade
{
    public JsonSerializerFacade(params Type[] objectTypes)
    {
        this.objectTypes = objectTypes;
        mapper = new MessageMapper();
        var settings = new SettingsHolder();
        serializer = new JsonSerializer().Configure(settings)(mapper);
        mapper.Initialize(objectTypes);
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
        return serializer.Deserialize(stream, objectTypes);
    }

    public object CreateInstance(Type type)
    {
        return type.IsInterface ? mapper.CreateInstance(type) : Activator.CreateInstance(type);
    }

    MessageMapper mapper;
    IMessageSerializer serializer;
    Type[] objectTypes;
}