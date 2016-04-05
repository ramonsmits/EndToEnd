namespace Tests.Tools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Tests.Permutations;
    using Variables;

    public abstract class TestEnvironment
    {
        PermutationDirectoryResolver resolver;

        protected TestEnvironment()
        {
            resolver = new PermutationDirectoryResolver(".");
        }

        public IEnumerable<Permutation> Generate()
        {
            TestsGlobal.CleanupAfterPreviousRuns();

            var permutations = CreatePermutations().ToArray();
            CreateTestEnvironments(permutations);

            return permutations;
        }

        protected abstract IEnumerable<Permutation> CreatePermutations();

        void CreateTestEnvironments(Permutation[] permutations)
        {
            var tasks = new List<Task>();

            foreach (var permutation in permutations)
            {
                var packageToUse = permutation;

                var task = Task.Factory.StartNew(() =>
                {
                    CreateEnvironment(TestsGlobal.BinDirectoryTemplate, packageToUse);
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        TestDescriptor CreateEnvironment(string startupDirTemplate, Permutation permutation)
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