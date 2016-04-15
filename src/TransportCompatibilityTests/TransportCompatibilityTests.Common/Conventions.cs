namespace TransportCompatibilityTests.Common
{
    using System;
    using System.IO;

    public class Conventions
    {
        public static Func<EndpointDefinition, int, string> AssemblyNameResolver =
            (definition, version) => $"{definition.TransportName}V{version}";

        public static Func<EndpointDefinition, int, string> AssemblyDirectoryResolver =
            (definition, version) =>
            {
                // ReSharper disable once RedundantAssignment
                var configuration = "Release";

                #if DEBUG
                    configuration = "Debug";
                #endif

                var assemblyName = AssemblyNameResolver(definition, version);
                return $"..\\..\\..\\{assemblyName}\\bin\\{configuration}";
            };

        public static Func<EndpointDefinition, int, string> AssemblyPathResolver =
            (definition, version) =>
            {
                var assemblyName = AssemblyNameResolver(definition, version);
                var assemblyDirectory = new DirectoryInfo(AssemblyDirectoryResolver(definition, version));

                return Path.Combine(assemblyDirectory.FullName, assemblyName + ".dll");
            };

        public static Func<EndpointDefinition, int, string> EndpointFacadeConfiguratorTypeNameResolver =
            (definition, version) =>
            {
                var assemblyName = AssemblyNameResolver(definition, version);

                return $"{assemblyName}.EndpointFacade";
            };
    }
}
