namespace Common
{
    using System;
    using System.IO;

    public interface ISerializerFacade
    {
        void Serialize(Stream stream, object instance);
        object[] Deserialize(Stream stream);
        object CreateInstance(Type type);
    }
}