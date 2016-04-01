namespace Common.Runner.Nuget
{
    using System.Collections.Generic;

    public class PackageVersionComparer : IEqualityComparer<Package>
    {
        public bool Equals(Package x, Package y)
        {
            return x.Version == y.Version;
        }

        public int GetHashCode(Package obj)
        {
            return obj.Version.GetHashCode();
        }
    }
}