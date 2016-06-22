namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Infrastructure;
    using NServiceBus;
    using NServiceBus.Features;

    abstract class SqlScTest
    {
        Dictionary<Type, Func<ITransportDetails>> transportDetailActivations = new Dictionary<Type, Func<ITransportDetails>>
        {
            { typeof(SqlTransportDetails), () => new SqlTransportDetails("Data Source=.\\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True") }
        };

        protected ITransportDetails StartUp(Type transportDetailsType)
        {
            Console.WriteLine($"Creating test for {transportDetailsType.Name}");
            var transportDetails = ActivateInstanceOfTransportDetail(transportDetailsType);
            serviceControl =  StartServiceControl(transportDetails);

            return transportDetails;
        }

        protected async Task<EndpointProxy> CreateSqlEndpoint(string endpointName, ITransportDetails transportDetails, IContainer container)
        {
            var config = new EndpointConfiguration(endpointName);
            config.UsePersistence<InMemoryPersistence>();
            config.EnableInstallers();
            config.PurgeOnStartup(true);
            config.DisableFeature<SecondLevelRetries>();
            config.DisableFeature<FirstLevelRetries>();

            config.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(container));

            config.SendFailedMessagesTo("error");
            config.AuditProcessedMessagesTo("audit");

            transportDetails.ConfigureEndpoint(config);

            return new EndpointProxy(await Endpoint.Create(config), container);
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

            return factory.Start(transport, TestContext.CurrentContext.Test.Name);
        }

        protected static Type[] AllTransports()
        {
            var transports = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Any(i => i == (typeof(ITransportDetails))));

            return transports.ToArray();
        }

        ServiceControlInstance serviceControl;
        protected ServiceControlApi ServiceControl => serviceControl.Api;
    }
}