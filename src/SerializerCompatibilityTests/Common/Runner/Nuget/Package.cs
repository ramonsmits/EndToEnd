namespace Common.Runner.Nuget
{
    using System.IO;

    public class Package
    {
        public PackageInfo Info { get; set; }

        public string Version { get; set; }

        public FileInfo[] Files { get; set; }
    }
}