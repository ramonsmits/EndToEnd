namespace ServiceControlCompatibilityTests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    abstract class SqlScTest
    {
        Dictionary<Type, Func<ITransportDetails>> transportDetailActivations = new Dictionary<Type, Func<ITransportDetails>>
        {
            { typeof(SqlTransportDetails), () => new SqlTransportDetails("Data Source=.\\SQLEXPRESS;Initial Catalog=nservicebus;Integrated Security=True") }
        };

        [TearDown]
        public void TearDown()
        {
            // TODO: Use transport to Clean Up

            serviceControl?.Stop();
        }

        protected ITransportDetails ActivateInstanceOfTransportDetail(Type transportDetailType)
        {
            Func<ITransportDetails> activator;
            transportDetailActivations.TryGetValue(transportDetailType, out activator);

            return activator?.Invoke();
        }

        protected void StartServiceControl(ITransportDetails transport)
        {
            // TODO: This is a build artifact from the latest SC build. It should be imported into the working folder of the build
            var factory = new ServiceControlFactory(@"C:\Temp\ServiceControl");

            serviceControl = factory.Start(transport, TestContext.CurrentContext.Test.Name);
        }

        protected ServiceControlApi ServiceControl => serviceControl.Api;
        protected ServiceControlInstance serviceControl;
    }
}