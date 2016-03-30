namespace Utils.Runner.Nuget
{
    using System.Collections.Generic;

    public class PackageVersionComparer : IEqualityComparer<Package>
    {
        public static PackageVersionComparer Default => new PackageVersionComparer();

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