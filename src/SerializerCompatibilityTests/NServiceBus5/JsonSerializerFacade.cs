using System;
using System.IO;
using Common;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Serializers.Json;

class JsonSerializerFacade : ISerializerFacade
{
    public JsonSerializerFacade(params Type[] objectTypes)
    {
        this.objectTypes = objectTypes;
        mapper = new MessageMapper();
        serializer = new JsonMessageSerializer(mapper);
        mapper.Initialize(objectTypes);
    }

    public void Serialize(Stream stream, object instance)
    {
        serializer.Serialize(instance, stream);
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
    JsonMessageSerializer serializer;
    Type[] objectTypes;
}