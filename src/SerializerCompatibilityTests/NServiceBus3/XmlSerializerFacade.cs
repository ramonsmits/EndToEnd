using System;
using System.IO;
using Common;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Serializers.XML;

class XmlSerializerFacade : ISerializerFacade
{
    public XmlSerializerFacade(params Type[] objectTypes)
    {
        mapper = new MessageMapper();
        serializer = new XmlMessageSerializer(mapper);
        mapper.Initialize(objectTypes);
        serializer.Initialize(objectTypes);
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

    readonly XmlMessageSerializer serializer;
    readonly MessageMapper mapper;
}