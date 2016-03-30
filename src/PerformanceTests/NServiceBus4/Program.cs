namespace NServiceBus4
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using Common;
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Unicast.Transport;
    using Utils;
    using NServiceBus.Installation.Environments;

    class Program
    {
        static string endpointName = "PerformanceTests_" + AppDomain.CurrentDomain.FriendlyName.Replace(' ', '_');

        static void Main(string[] args)
        {
            LoadCorrectEncryptionConfiguration();

            //HACK: for trial dialog issue https://github.com/Particular/NServiceBus/issues/2001
            var synchronizationContext = SynchronizationContext.Current;
            var options = BusCreationOptions.Parse(args);
            var bus = CreateBus(options);
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
            TestRunner.EndpointName = endpointName;

            TestRunner.RunTests(bus, options);
        }

        private static void LoadCorrectEncryptionConfiguration()
        {
            var nsbAssembly = Assembly.Load(new AssemblyName("NServiceBus.Core"));
            var version = Version.Parse(FileVersionInfo.GetVersionInfo(nsbAssembly.Location).FileVersion);
            if (version >= Version.Parse("4.7.8"))
            {
                var newConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_47.config");
                Debug.Assert(File.Exists(newConfig));
                AppConfigReplacer.Replace(newConfig);
            }
        }

        static IBus CreateBus(BusCreationOptions options)
        {
            if (options.Cleanup)
            {
                QueueUtils.DeleteQueuesForEndpoint(endpointName);
            }

            TransportConnectionString.Override(() => "deadLetter=false;journal=false");
            TransportConfigOverride.MaximumConcurrencyLevel = options.NumberOfThreads;

            var configure = Configure.With()
                .DefineEndpointName(endpointName)
                .DefaultBuilder();

            //Configure.Features.Disable<Audit>();
            configure.RijndaelEncryptionService();

            switch (options.Serialization)
            {
                case SerializationKind.Xml:
                    Configure.Serialization.Xml();
                    break;
                case SerializationKind.Json:
                    Configure.Serialization.Json();
                    break;
                case SerializationKind.Bson:
                    Configure.Serialization.Bson();
                    break;
                case SerializationKind.Bin:
                    Configure.Serialization.Binary();
                    break;
            }

            if (options.Saga)
                Configure.Features.Enable<Sagas>();

            if (options.Transport == TransportKind.Msmq)
                configure.UseTransport<Msmq>();

            if (options.Persistence == PersistenceKind.InMemory || options.Volatile)
                Configure.Endpoint.AsVolatile();

            if (options.SuppressDTC)
                Configure.Transactions.Advanced(settings => settings.DisableDistributedTransactions());

            var startableBus = configure.InMemoryFaultManagement().UnicastBus().CreateBus();

            return startableBus.Start(
                () => Configure.Instance.ForInstallationOn<Windows>().Install());
        }
    }
}