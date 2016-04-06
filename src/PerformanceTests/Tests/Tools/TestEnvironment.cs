namespace Tests.Tools
{
    using System.IO;
    using Tests.Permutations;
    using Variables;

    public class TestEnvironment
    {
        PermutationDirectoryResolver resolver;

        public TestEnvironment()
        {
            resolver = new PermutationDirectoryResolver(".");
        }

        public TestDescriptor CreateTestEnvironments(Permutation permutation)
        {
            return CreateEnvironment(TestsGlobal.BinDirectoryTemplate, permutation);
        }

        TestDescriptor CreateEnvironment(string startupDirTemplate, Permutation permutation)
        {
            startupDirTemplate = Path.Combine("permutations", startupDirTemplate);
            var id = DeterministicUuid.Create(permutation.ToString());
            var result = resolver.Resolve(permutation);
            var startupDir = GetStartupDir(startupDirTemplate, permutation.Version, id.ToString());

            if (startupDir.Exists)
            {
                startupDir.Delete(true);
            }

            startupDir.Create();

            var sourceAssemblyFiles = Directory.GetFiles(result.RootProjectDirectory, "*");
            CopyAssembliesToStarupDir(startupDir, sourceAssemblyFiles, result.Files);


            var projectAssemblyPath = Path.Combine(startupDir.FullName, result.RootProjectDirectory + ".exe");

            var descriptor = new TestDescriptor
            {
                Permutation = permutation,
                ProjectAssemblyPath = projectAssemblyPath,
            };

            permutation.Exe = projectAssemblyPath;

            GenerateBat(descriptor);

            return descriptor;
        }

        void GenerateBat(TestDescriptor value)
        {
            var args = PermutationParser.ToArgs(value.Permutation);
            var exe = new FileInfo(value.ProjectAssemblyPath);
            var batFile = Path.Combine(exe.DirectoryName, "start.bat");

            if (!File.Exists(batFile)) File.WriteAllText(batFile, exe.Name + " " + args);
        }

        void CopyAssembliesToStarupDir(DirectoryInfo destination, string[] baseFiles, FileInfo[] overrides)
        {
            foreach (var file in baseFiles)
            {
                var dst = Path.Combine(destination.FullName, Path.GetFileName(file));
                Clone(file, dst);
            }

            foreach (var @override in overrides)
            {
                var dst = Path.Combine(destination.FullName, @override.Name);
                Clone(@override.FullName, dst);
            }
        }

        static void Clone(string src, string dst)
        {
            src = Path.GetFullPath(src);
            if (File.Exists(dst)) return;
            if (!SymbolicLink.Create(src, dst))
                File.Copy(src, dst);
        }

        static DirectoryInfo GetStartupDir(string codeBaseDirTemplate, NServiceBusVersion version, string id)
        {
            var directoryName = string.Format(codeBaseDirTemplate, version, id);
            return new DirectoryInfo(directoryName);
        }
    }
}
