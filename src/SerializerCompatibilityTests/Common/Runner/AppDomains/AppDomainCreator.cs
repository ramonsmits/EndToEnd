namespace Common.Runner.AppDomains
{
    using System;
    using System.IO;
    using System.Reflection;
    using Nuget;
    using Paket;

    [Serializable]
    public class AppDomainCreator
    {
        public AppDomainDescriptor CreateDomain(string startupDirTemplate, Package package)
        {
            var startupDir = CreateStartupDir(startupDirTemplate, package.Version, Guid.NewGuid());

            var sourceAssemblyDir = package.Info.PackageName + SemVer.Parse(package.Version).Major;
            var sourceAssemblyFiles = Directory.GetFiles(sourceAssemblyDir, "*");

            CopyAssembliesToStarupDir(startupDir, sourceAssemblyFiles, package.Files);

            var appDomain = AppDomain.CreateDomain(
                $"{package.Info.PackageName} {package.Version}",
                null,
                new AppDomainSetup
                {
                    ApplicationBase = startupDir.FullName
                });

            SetupBindingRedirects(package, appDomain);

            appDomain.SetData("FriendlyName", appDomain.FriendlyName);

            return new AppDomainDescriptor
            {
                AppDomain = appDomain,
                ProjectAssemblyPath = Path.Combine(startupDir.FullName, sourceAssemblyDir + ".dll"),
                PackageVersion = package.Version
            };
        }

        static void SetupBindingRedirects(Package package, AppDomain appDomain)
        {
            var assemblyRedirector = new CustomAssemblyLoader(appDomain);

            foreach (var assembly in package.Files)
            {
                var assemblyName = AssemblyName.GetAssemblyName(assembly.FullName);
                var token = assemblyName.GetPublicKeyToken();
                var shortName = assemblyName.Name;
                var version = assemblyName.Version;

                assemblyRedirector.AddAssemblyRedirect(shortName, version, token);
            }
        }

        void CopyAssembliesToStarupDir(DirectoryInfo destination, string[] baseFiles, FileInfo[] overrides)
        {
            foreach (var file in baseFiles)
            {
                var newFilename = Path.Combine(destination.FullName, Path.GetFileName(file));

                File.Copy(file, newFilename);
            }

            foreach (var @override in overrides)
            {
                @override.CopyTo(Path.Combine(destination.FullName, @override.Name), true);
            }
        }

        static DirectoryInfo CreateStartupDir(string codeBaseDirTemplate, string version, Guid uniqueId)
        {
            var directoryName = string.Format(codeBaseDirTemplate, version, uniqueId);

            if (Directory.Exists(directoryName) == false)
            {
                return Directory.CreateDirectory(directoryName);
            }

            return new DirectoryInfo(directoryName);
        }
    }

    public class AppDomainDescriptor
    {
        public AppDomain AppDomain { get; set; }
        public string ProjectAssemblyPath { get; set; }
        public string PackageVersion { get; set; }
    }
}