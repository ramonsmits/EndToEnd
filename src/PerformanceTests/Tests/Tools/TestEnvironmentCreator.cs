namespace Tests.Tools
{
    using System;
    using System.IO;
    using Categories;
    using Utils.Runner.AppDomains;
    using Variables;

    public class TestEnvironmentCreator
    {
        PermutationDirectoryResolver resolver;

        public TestEnvironmentCreator(string rootDirectory)
        {
            resolver = new PermutationDirectoryResolver(rootDirectory);
        }

        public TestDescriptor CreateEnvironment(string startupDirTemplate, Permutation permutation)
        {
            var result = resolver.Resolve(permutation);

            var startupDir = CreateStartupDir(startupDirTemplate, permutation.Version, Guid.NewGuid());

            var sourceAssemblyFiles = Directory.GetFiles(result.RootProjectDirectory, "*");

            CopyAssembliesToStarupDir(startupDir, sourceAssemblyFiles, result.Files);

            var projectAssemblyPath = Path.Combine(startupDir.FullName, result.RootProjectDirectory + ".exe");

            return new TestDescriptor
            {
                ProjectAssemblyPath = projectAssemblyPath,
            };
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

        static DirectoryInfo CreateStartupDir(string codeBaseDirTemplate, NServiceBusVersion version, Guid uniqueId)
        {
            var directoryName = string.Format(codeBaseDirTemplate, version, uniqueId);

            return Directory.CreateDirectory(directoryName);
        }
    }
}