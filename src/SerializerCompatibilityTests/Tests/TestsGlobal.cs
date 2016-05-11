namespace Tests
{
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [SetUpFixture]
    public class TestsGlobal
    {
        [SetUp]
        public void RunsBeforeAnyTest()
        {
            CleanupAfterPreviousRuns();
        }

        public static void CleanupAfterPreviousRuns()
        {
            if (CleanupDone == false)
            {
                RemoveAppDomainCodeBaseDirs();
            }

            CleanupDone = true;
        }

        static void RemoveAppDomainCodeBaseDirs()
        {
            new DirectoryInfo(".")
                .GetDirectories(BinDirectorySearchPattern).ToList()
                .ForEach(d => d.Delete(true));
        }

        public static string BinDirectoryTemplate = "NServiceBus_{0}_{1}";

        static string BinDirectorySearchPattern = string.Format(BinDirectoryTemplate, "*", "*");
        static bool CleanupDone;
    }
}