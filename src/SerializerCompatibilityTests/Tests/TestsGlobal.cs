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
            ClenupAfterPreviousRuns();
        }

        public static void ClenupAfterPreviousRuns()
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
                .ForEach(d => d.Delete(true));
        }

        public static string BinDirectoryTemplate = "NServiceBus_{0}_{1}";

        static string BinDriectorySearchPattern = string.Format(BinDirectoryTemplate, "*", "*");
        static bool CleanupDone;
    }
}