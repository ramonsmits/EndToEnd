namespace ServiceControlCompatibilityTests
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Xml.Linq;

    class ServiceControlFactory
    {
        readonly string unzipPath;

        public ServiceControlFactory(string unzipPath)
        {
            this.unzipPath = unzipPath;
        }

        public ServiceControlInstance Start(ITransportDetails transportDetails, string dataSlug)
        {
            var rawScPath = Path.Combine(unzipPath, "ServiceControl");
            var transportPath = Path.Combine(unzipPath, "Transports", transportDetails.TransportName);
            var dataPath = Path.Combine(unzipPath, "Data", dataSlug);
            var installPath = Path.Combine(unzipPath, "Install");

            Console.WriteLine($"Raw Service Control path: {rawScPath}");
            Console.WriteLine($"Transport path: {transportPath}");
            Console.WriteLine($"Data path: {dataPath}");
            Console.WriteLine($"Install path: {installPath}");

            Directory.CreateDirectory(dataPath);
            if (Directory.Exists(installPath))
            {
                Console.WriteLine("Creating installation path");
                Directory.Delete(installPath, true);
            }

            Copy(rawScPath, installPath);
            Copy(transportPath, installPath);

            const string hostName = "localhost";
            var port = FindAvailablePort(33333);

            WriteAppConfig(installPath, hostName, port, transportDetails, dataPath);

            var serviceControlInstance = new ServiceControlInstance(installPath, $"http://{hostName}:{port}/api");
            serviceControlInstance.Start();

            return serviceControlInstance;
        }

        static void WriteAppConfig(string installPath, string hostName, int port, ITransportDetails transportDetails, string dataPath)
        {
            Console.WriteLine("Writing config values");

            var exeMapping = new ExeConfigurationFileMap { ExeConfigFilename = Path.Combine(installPath, "ServiceControl.exe.config") };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(exeMapping, ConfigurationUserLevel.None);
            var settings = configuration.AppSettings.Settings;
            settings.Set(SettingsList.Port, port.ToString());
            settings.Set(SettingsList.HostName, hostName);
            settings.Set(SettingsList.LogPath, Path.Combine(installPath, "Logs"));
            settings.Set(SettingsList.DBPath, dataPath);
            settings.Set(SettingsList.ForwardAuditMessages, "false");
            settings.Set(SettingsList.ForwardErrorMessages, "false");
            settings.Set(SettingsList.AuditQueue, "audit");
            settings.Set(SettingsList.ErrorQueue, "error");
            settings.Set(SettingsList.AuditRetentionPeriod, TimeSpan.FromHours(720).ToString());
            settings.Set(SettingsList.ErrorRetentionPeriod, TimeSpan.FromDays(15).ToString());

            // Add Settings for performance tuning 
            // See https://github.com/Particular/ServiceControl/issues/655
            if (!settings.AllKeys.Contains("Raven/Esent/MaxVerPages"))
            {
                settings.Add("Raven/Esent/MaxVerPages", "2048");
            }
            UpdateRuntimeSection(configuration);

            transportDetails.ApplyTo(configuration);
            transportDetails.EnsurePrerequisites();

            Console.WriteLine("Saving config values");
            configuration.Save();
            Console.WriteLine("Config values saved");
        }

        static void UpdateRuntimeSection(Configuration configuration)
        {
            var runtimesection = configuration.GetSection("runtime");
            var runtimeXml = XDocument.Parse(runtimesection.SectionInformation.GetRawXml() ?? "<runtime/>");

            // Set gcServer Value if it does not exist
            var gcServer = runtimeXml.Descendants("gcServer").SingleOrDefault();
            if (gcServer == null)  //So no config so we can set 
            {
                gcServer = new XElement("gcServer");
                gcServer.SetAttributeValue("enabled", "true");
                if (runtimeXml.Root != null)
                {
                    runtimeXml.Root.Add(gcServer);
                    runtimesection.SectionInformation.SetRawXml(runtimeXml.Root.ToString());
                }
            }
        }

        static int FindAvailablePort(int startPort)
        {
            Console.WriteLine("Finding available port");

            var activeTcpListeners = IPGlobalProperties
                .GetIPGlobalProperties()
                .GetActiveTcpListeners();

            for (var port = startPort; port < startPort + 1024; port++)
            {
                var portCopy = port;
                if (activeTcpListeners.All(endPoint => endPoint.Port != portCopy))
                {
                    Console.WriteLine($"Port found at {port}");
                    return port;
                }
            }

            Console.WriteLine($"No available port found. Using startport ({startPort})");
            return startPort;
        }

        #region File Utils

        static void Copy(string sourceDirectory, string targetDirectory)
        {
            Console.WriteLine($"Copying files from {sourceDirectory} to {targetDirectory}");

            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }

        #endregion
    }
}