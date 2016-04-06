namespace Categories
{
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;

    public class Base
    {
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
            permutation.Tests = new[] { memberName };
            var environment = new TestEnvironment();
            environment.CreateTestEnvironments(permutation);
            Invoke(permutation);
        }

        static void Invoke(Permutation permutation)
        {
            var fi = new FileInfo(permutation.Exe);
            var pi = new ProcessStartInfo(permutation.Exe, PermutationParser.ToArgs(permutation))
            {
                UseShellExecute = false,
                WorkingDirectory = fi.DirectoryName,
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
    }
}
