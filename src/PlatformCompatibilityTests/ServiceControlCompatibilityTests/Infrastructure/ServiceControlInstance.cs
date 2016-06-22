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
            Console.WriteLine("Starting ServiceControl");

            var psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = installFolder,
                FileName = Path.Combine(installFolder, "ServiceControl.exe"),
            };

            process = Process.Start(psi);

            if (process == null)
            {
                throw new Exception("The process is null which means it hasn't actually started");
            }


            // TODO: This should probably by async and eventually give up
            var retryCount = 0;
            while (!Api.CheckIsAvailable() && retryCount++ < 20)
            {
                Console.WriteLine("ServiceControl not available yet, waiting 200ms");
                Thread.Sleep(TimeSpan.FromMilliseconds(200));
            }

            if (retryCount >= 20)
            {
                throw new ApplicationException("Could not start Service Control after 20 attempts");
            }

            Console.WriteLine("Service Control successfully started");
        }

        public void Stop()
        {
            process?.Kill();
            process?.WaitForExit(5000);
        }

        string installFolder;
        Process process;
    }
}