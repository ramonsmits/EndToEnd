using System;
using System.IO;
using Common;
using NServiceBus;
using NServiceBus.MessageInterfaces.MessageMapper.Reflection;
using NServiceBus.Serialization;
using NServiceBus.Settings;

class XmlSerializerFacade : ISerializerFacade
{
    public XmlSerializerFacade(params Type[] objectTypes)
    {
        mapper = new MessageMapper();
        var settings = new SettingsHolder();
        var conventions = CreateTestConventions(settings);
        settings.Set<Conventions>(conventions);
        settings.Set("TypesToScan", objectTypes);

        serializer = new XmlSerializer().Configure(settings)(mapper);
        mapper.Initialize(objectTypes);
    }


    public void Serialize(Stream stream, object instance)
    {
        serializer.Serialize(instance, stream);
    }

    public object[] Deserialize(Stream stream)
    {
        return serializer.Deserialize(stream);
    }

    public object CreateInstance(Type type)
    {
        return type.IsInterface ? mapper.CreateInstance(type) : Activator.CreateInstance(type);
    }

    object CreateTestConventions(SettingsHolder settings)
    {
        var builder = new ConventionsBuilder(settings);
        builder.DefiningMessagesAs(type => type.FullName.Contains("TestCases"));
        return builder.Conventions;
    }

    IMessageSerializer serializer;
    MessageMapper mapper;
}