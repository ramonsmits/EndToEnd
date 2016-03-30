namespace Common.Runner.Nuget
{
    using System;

    [Serializable]
    public class PackageInfo
    {
        public PackageInfo(string packageName, string versionConstraint = null)
        {
            PackageName = packageName;
            VersionConstraint = versionConstraint;
        }

        public string PackageName { get; }
        public string VersionConstraint { get; }

        public static implicit operator PackageInfo(string package)
        {
            var pair = package.Split(':');
            if (pair.Length == 2)
                return new PackageInfo(pair[0], pair[1]);

            throw new ArgumentException($"The value '{package}' is not in a correct format. Use 'PackageName:VersionConstraint[:pre]'.");
        }

        public override string ToString()
        {
            return $"{PackageName} {VersionConstraint}";
        }
    }
}