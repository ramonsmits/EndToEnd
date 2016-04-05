namespace Tests.Tools
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Categories;
    using Variables;

    public class PermutationDirectoryResolver
    {
        static string[] assembliesToSkip = { "NServiceBus.Core.dll" };
        readonly string rootDirectory;

        public PermutationDirectoryResolver(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        public IEnumerable<PermutationResult> Resolve(Permutation permutation)
        {
            var components = GetPermutationComponents(permutation);

            foreach (var component in components)
            {
                var di = new DirectoryInfo(Path.Combine(rootDirectory, component));
                if (di.Exists)
                {
                    yield return new PermutationResult(component)
                    {
                        Files = di.GetFiles("*.dll").Where(info => !Array.Exists(assembliesToSkip, f => f == info.Name)).ToArray()
                    };
                }
            }
        }

        IEnumerable<string> GetPermutationComponents(Permutation permutation)
        {
            if (permutation.Persister != Persistence.InMemory)
            {
                yield return $"Persistence.{permutation.Version}.{permutation.Persister}";
            }
        }

        public class PermutationResult
        {
            public PermutationResult(string component)
            {
                ComponentName = component;
            }

            public FileInfo[] Files { get; set; }
            public string ComponentName { get; private set; }
        }
    }
}