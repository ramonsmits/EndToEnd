namespace TransportCompatibilityTests.Common
{
    using System;
    using System.IO;

    public class EndpointFacadeBuilder
    {
        public static IEndpointFacade CreateAndConfigure<TEndpointDefinition>(TEndpointDefinition endpointDefinition, int version)
           where TEndpointDefinition : EndpointDefinition
        {
            var startupDirectory = new DirectoryInfo(Conventions.AssemblyDirectoryResolver(endpointDefinition, version));

            var appDomain = AppDomain.CreateDomain(
                startupDirectory.Name,
                null,
                new AppDomainSetup
                {
                    ApplicationBase = startupDirectory.FullName,
                    ConfigurationFile = Path.Combine(startupDirectory.FullName, $"{endpointDefinition.TransportName}V{version}.dll.config")
                });

            var assemblyPath = Conventions.AssemblyPathResolver(endpointDefinition, version);
            var typeName = Conventions.EndpointFacadeConfiguratorTypeNameResolver(endpointDefinition, version);

            var facade = (IEndpointFacade)appDomain.CreateInstanceFromAndUnwrap(assemblyPath, typeName);
            facade.Bootstrap(endpointDefinition);

            return facade;
        }

    }
}
