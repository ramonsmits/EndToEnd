namespace ServiceControlCompatibilityTests
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    class ServiceControlInstance
    {
        public ServiceControlInstance(string installFolder, string rootUri)
        {
            this.installFolder = installFolder;
            Api = new ServiceControlApi(rootUri);
        }

        public ServiceControlApi Api { get; }

        public void Start()
        {
            var psi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = installFolder,
                FileName = Path.Combine(installFolder, "ServiceControl.exe")
            };

            process = Process.Start(psi);

            // TODO: This should probably by async and eventually give up
            while (!Api.CheckIsAvailable())
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }
        }

        public void Stop()
        {
            process?.Kill();
            process?.WaitForExit();
        }

        string installFolder;
        Process process;
    }
}