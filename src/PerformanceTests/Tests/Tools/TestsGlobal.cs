namespace Tests.Tools
{
    using System.IO;
    using System.Linq;
    using NUnit.Framework;

    [SetUpFixture]
    public class TestsGlobal
    {
        public static string BinDirectoryTemplate = "NServiceBus_{0}_{1}";

        private static string BinDriectorySearchPattern = string.Format(BinDirectoryTemplate, "*", "*");
        private static bool CleanupDone;

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
                .GetDirectories(BinDriectorySearchPattern).ToList()
                .ForEach(d => 
                {
                    try
                    {
                        d.Delete(true);
                    }
                    catch { }
                });
        }
    }
}