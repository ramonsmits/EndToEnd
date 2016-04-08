namespace Categories
{
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;
    using VisualStudioDebugHelper;
    using Variables;
    using System;
    using System.Linq;

    public class Base
    {
        static readonly bool InvokeEnabled = bool.Parse(ConfigurationManager.AppSettings["InvokeEnabled"]);

        public virtual void GatedSendLocalRunner(Permutation permutation)
        {
            Tasks(permutation);
        }

        public virtual void SendLocalOneOnOneRunner(Permutation permutation)
        {
            Tasks(permutation);
        }

        void Tasks(Permutation permutation, [CallerMemberName] string memberName = "")
        {
            permutation.Category = GetType().Name;
            permutation.Tests = new[] { memberName };
            var environment = new TestEnvironment();
            environment.CreateTestEnvironments(permutation);
            Invoke(permutation);
        }

        static void Invoke(Permutation permutation)
        {
            if (!InvokeEnabled) Assert.Inconclusive("Invoke disabled, set 'InvokeEnabled' appSetting to True.");

            var processId = DebugAttacher.GetCurrentVisualStudioProcessId();
            var processIdArgument = processId >= 0 ? string.Format(" --processId={0}", processId) : string.Empty;

            var exe = new FileInfo(permutation.Exe);

            Patch(exe, permutation.Platform);

            var pi = new ProcessStartInfo(exe.FullName, PermutationParser.ToArgs(permutation) + processIdArgument)
            {
                UseShellExecute = false,
                WorkingDirectory = exe.DirectoryName,
            };


            using (var p = Process.Start(pi))
            {
                if (!p.WaitForExit(70000))
                {
                    p.Kill();
                    Assert.Fail("Killed!");

                }
                Assert.AreEqual(0, p.ExitCode, "Execution failed.");
            }
        }

        static void Patch(FileInfo exe, Platform platform)
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            var corflagsPath = Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "corflags.exe", SearchOption.AllDirectories).First();
            var args = exe.Name + " /32bit" + (platform == Platform.x64 ? "-" : "+");

            var pi = new ProcessStartInfo(corflagsPath, args)
            {
                UseShellExecute = false,
                WorkingDirectory = exe.DirectoryName,
            };

            using (var p = Process.Start(pi))
            {
                p.WaitForExit();
            }

        }
    }
}
