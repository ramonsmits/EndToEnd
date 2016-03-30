namespace Common.Tests.TestCases.Types
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PolymorphicCollection
    {
        public List<BaseEntity> Items { get; set; }
    }

    [Serializable]
    public class BaseEntity
    {
        public virtual string Name { get; set; }
    }

    [Serializable]
    public class SpecializationA : BaseEntity
    {
        public override string Name { get; set; }

        public override bool Equals(object obj)
        {
            var other = (SpecializationA)obj;

            return other != null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }

    [Serializable]
    public class SpecializationB : BaseEntity
    {
        public override string Name { get; set; }

        public override bool Equals(object obj)
        {
            var other = (SpecializationB)obj;

            return other != null && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}