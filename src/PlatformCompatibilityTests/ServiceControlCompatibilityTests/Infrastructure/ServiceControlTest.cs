namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    abstract class SqlScTest
    {
        Dictionary<Type, Func<ITransportDetails>> transportDetailActivations = new Dictionary<Type, Func<ITransportDetails>>
        {
            { typeof(SqlTransportDetails), () => new SqlTransportDetails("Data Source=.\\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True") },
            { typeof(MsmqTransportDetails), () => new MsmqTransportDetails() },
            { typeof(RabbitMQTransportDetails), () => new RabbitMQTransportDetails("host=localhost") },
            { typeof(ASBEndpointTopologyTransportDetails), () => new ASBEndpointTopologyTransportDetails(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString")) },
            { typeof(ASBForwardingTopologyTransportDetails), () => new ASBForwardingTopologyTransportDetails(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString")) },
            { typeof(ASQTransportDetails), () => new ASQTransportDetails(Environment.GetEnvironmentVariable("AzureStorageQueueTransport.ConnectionString")) },
        };

        protected IEndpointFactory StartUp(Type transportDetailsType)
        {
            Console.WriteLine($"Creating test for {transportDetailsType.Name}");
            var transportDetails = ActivateInstanceOfTransportDetail(transportDetailsType);
            serviceControl =  StartServiceControl(transportDetails);

            var endpointFactory = new EndpointFactory(transportDetails);

            return endpointFactory;
        }

        [TearDown]
        public void ShutDown()
        {
            // TODO: Clean up using the transportDetails
            serviceControl?.Stop();
        }

        ITransportDetails ActivateInstanceOfTransportDetail(Type transportDetailType)
        {
            var activator = transportDetailActivations[transportDetailType];

            return activator?.Invoke();
        }

        ServiceControlInstance StartServiceControl(ITransportDetails transport)
        {
            var runningInTeamCity = Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;
            
            Console.WriteLine(runningInTeamCity ? "Running in TeamCity" : "Running outside of TeamCity");
            
            // TODO: If Not Running in TeamCity get this from a different Env variable?
            var serviceControlPath = runningInTeamCity 
                ? Path.Combine(Environment.CurrentDirectory, "ServiceControl")
                : @"C:\Temp\ServiceControl";

            Console.WriteLine($"Creating SC Factory at {serviceControlPath}");
            var factory = new ServiceControlFactory(serviceControlPath);

            return factory.Start(transport, NUnit.Framework.TestContext.CurrentContext.Test.Name);
        }

        protected static Type[] AllTransports()
        {
            var assemblyToLoad = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            foreach (var path in assemblyToLoad)
            {
                AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));
            }

            var transports = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Any(i => i == (typeof(ITransportDetails))));

            return transports.ToArray();
        }

        ServiceControlInstance serviceControl;
        protected ServiceControlApi ServiceControl => serviceControl.Api;
    }
}