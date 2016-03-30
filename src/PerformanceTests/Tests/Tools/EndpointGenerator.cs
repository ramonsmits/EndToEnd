namespace Tests.Tools
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Utils.Runner.AppDomains;
    using Utils.Runner.Nuget;

    public class EndpointGenerator
    {
        private static string PackageStore = "../..";

        private static string NugetFeed = "https://www.myget.org/F/particular";

        private static PackageInfo[] PackageInfos =
        {
            // latest v3 (3.3)
            "NServiceBus:(3.0,4.0)",

            // v4 (4.0 - 4.7)
            "NServiceBus:(4.0,4.1)",
            "NServiceBus:(4.1,4.2)",
            "NServiceBus:(4.2,4.3)",
            "NServiceBus:(4.3,4.4)",
            "NServiceBus:(4.4,4.5)",
            "NServiceBus:(4.5,4.6)",
            "NServiceBus:(4.6,4.7)",
            "NServiceBus:(4.0,5.0)",

            // v5 (5.0 - 5.2)
            "NServiceBus:(5.0,5.1)",
            "NServiceBus:(5.1,5.2)",
            "NServiceBus:(5.2,5.3)",
            "NServiceBus:(5.0,6.0)",

            // v6
            "NServiceBus:(6.0-prerelease,6.1-prerelease)"
        };

        public IEnumerable<TestInfo> Generate()
        {
            TestsGlobal.CleanupAfterPreviousRuns();

            var packages = DownloadPackages(PackageInfos);

            var runners = CreateAppDomainRunners(packages);

            return packages.Select(p => new TestInfo
            {
                RunPackage = p,
                Runner = runners[p.Version]
            });
        }

        private Package[] DownloadPackages(IEnumerable<PackageInfo> packageInfos)
        {
            var resolver = new NuGetPackageResolver(PackageStore, NugetFeed);
            var packages = new ConcurrentBag<Package>();

            foreach (var packageInfo in packageInfos)
            {
                var package = resolver.DownloadPackageWithDependencies(packageInfo).GetAwaiter().GetResult();

                packages.Add(package);
            }

            var uniquePackages = RemoveVersionDuplicates(packages.ToList());

            return uniquePackages;
        }

        private Package[] RemoveVersionDuplicates(IEnumerable<Package> availableNServiceBusVersions)
        {
            //HINT: We want to remove packageinfos which resolved to the same version number. E.g.
            //      (5.2,5.3) and (5.0,6.0) could both resolve to 5.2.8 if that's the newest version.
            return availableNServiceBusVersions.DistinctBy(package => package.Version).ToArray();
        }

        private static Dictionary<string, AppDomainRunner> CreateAppDomainRunners(Package[] packages)
        {
            var appDomainCreator = new AppDomainCreator();
            var appDomainRunners = new ConcurrentBag<AppDomainRunner>();
            var tasks = new List<Task>();

            foreach (var package in packages)
            {
                var packageToUse = package;

                var task = Task.Factory.StartNew(() =>
                {
                    var domain = appDomainCreator.CreateDomain(TestsGlobal.BinDirectoryTemplate, packageToUse);
                    var runner = new AppDomainRunner(domain);

                    appDomainRunners.Add(runner);
                });

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            return appDomainRunners.ToDictionary(i => i.PackageVersion, i => i);
        }
    }
}