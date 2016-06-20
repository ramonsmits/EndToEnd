namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    abstract class SqlScTest
    {
        Dictionary<Type, Func<ITransportDetails>> transportDetailActivations = new Dictionary<Type, Func<ITransportDetails>>
        {
            { typeof(SqlTransportDetails), () => new SqlTransportDetails("Data Source=.\\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True") }
        };

        protected void StartUp(Type transportDetailsType)
        {
            var transportDetails = ActivateInstanceOfTransportDetail(transportDetailsType);
            serviceControl =  StartServiceControl(transportDetails);
        }

        [TearDown]
        public void ShutDown()
        {
            // TODO: Clean up using the transportDetails
            serviceControl?.Stop();
        }

        ITransportDetails ActivateInstanceOfTransportDetail(Type transportDetailType)
        {
            Func<ITransportDetails> activator;
            transportDetailActivations.TryGetValue(transportDetailType, out activator);

            return activator?.Invoke();
        }

        ServiceControlInstance StartServiceControl(ITransportDetails transport)
        {
            var runningInTeamCity = Environment.GetEnvironmentVariable("TEAMCITY_VERSION") != null;
            
            Console.WriteLine(runningInTeamCity ? "Running in TeamCity" : "Running outside of TeamCity");
            
            // TODO: If Not Running in TeamCity get this from a different Env variable?
            var serviceControlPath = runningInTeamCity 
                ? Environment.CurrentDirectory
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