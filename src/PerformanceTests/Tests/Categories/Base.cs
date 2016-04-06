namespace Categories
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;

    public class Base
    {
        public virtual void SendLocal(Permutation permutation)
        {
            Tasks(permutation);
        }

        public virtual void PublishToSelf(Permutation permutation)
        {
            Tasks(permutation);
        }

        public virtual void SendToSelf(Permutation permutation)
        {
            Tasks(permutation);
        }

        void Tasks(Permutation permutation, [CallerMemberName] string memberName = "")
        {
            var environment = new TestEnvironment();
            environment.CreateTestEnvironments(permutation);
            CheckInPermutation(permutation, memberName);
            Invoke(permutation, memberName);
        }

        static void CheckInPermutation(Permutation permutation, string memberName)
        {
            if (!permutation.Tests.Contains(memberName)) Assert.Inconclusive("Not in category" + memberName);
        }

        static void Invoke(Permutation permutation, string memberName)
        {
            var fi = new FileInfo(permutation.Exe);
            var pi = new ProcessStartInfo(permutation.Exe, PermutationParser.ToArgs(permutation))
            {
                UseShellExecute = false,
                WorkingDirectory = fi.DirectoryName,
            };

            using (var p = Process.Start(pi))
            {
                Assert.IsTrue(p.WaitForExit(25000), "Process still running.");
                Assert.AreEqual(0, p.ExitCode, "Execution failed.");
            }
        }
    }
}