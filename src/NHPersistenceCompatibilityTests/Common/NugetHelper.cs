using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class NugetHelper
    {
        public IEnumerable<string> GetPossibleVersionsFor(string packageName, string minimumVersion)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            var packages = repo.FindPackagesById(packageName)
                .Where(p => !p.Version.ToNormalizedString().Contains("unstable"))
                .Where(p => p.Version.CompareTo(SemanticVersion.Parse(minimumVersion)) >= 0);

            return packages.Select(p => p.Version.ToString());
        }

        internal void DownloadPackageTo(string packageName, string version, string location)
        {
            var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");
            var packageManager = new PackageManager(repo, location);

            packageManager.InstallPackage(packageName, SemanticVersion.Parse(version));
        }
    }
}