using System;
using System.IO;

namespace Common
{
    public class Package
    {
        public PackageInfo Info { get; set; }

        public string Version { get; set; }

        public FileInfo[] Files { get; set; }
    }

    [Serializable]
    public class PackageInfo
    {
        public PackageInfo(string packageName, string version)
        {
            PackageName = packageName;
            Version = version;
        }

        public string PackageName { get; }
        public string Version { get; }
        public string FolderName => $"{PackageName}_{Version}";
        public string AssemblyName => $"{ToString()}.dll";

        public static implicit operator PackageInfo(string package)
        {
            var pair = package.Split('_');
            if (pair.Length == 2)
                return new PackageInfo(pair[0], pair[1]);

            throw new ArgumentException($"The value '{package}' is not in a correct format. Use 'PackageName_Version'.");
        }

        public override string ToString()
        {
            return $"{PackageName}_{Version}";
        }
    }
}