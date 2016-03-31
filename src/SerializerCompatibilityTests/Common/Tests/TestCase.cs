namespace Common.Tests
{
    using System;

    public abstract class TestCase : MarshalByRefObject
    {
        public abstract Type MessageType { get; }
        public virtual bool SupportsVersion(string version) => true;

        public virtual bool SupportsFormat(SerializationFormat format) => true;

        public abstract void Populate(object instance);

        public abstract void CheckIfAreEqual(object instanceA, object instanceB);
    }
}