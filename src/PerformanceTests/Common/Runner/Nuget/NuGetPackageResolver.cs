namespace Utils.Runner.Nuget
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.FSharp.Control;
    using Microsoft.FSharp.Core;
    using Paket;

    public class NuGetPackageResolver
    {
        private readonly string packagesStore;
        private readonly string nugetFeed;

        public NuGetPackageResolver(string packagesStore, string nugetFeed = Constants.DefaultNugetStream)
        {
            this.packagesStore = packagesStore;
            this.nugetFeed = nugetFeed;
        }

        public Task<Package> DownloadPackageWithDependencies(PackageInfo packageInfo)
        {
            return DownloadPackageWithDependencies(packageInfo, ResolverStrategy.Max);
        }

        private async Task<Package> DownloadPackageWithDependencies(PackageInfo packageInfo, ResolverStrategy resolverStrategy)
        {
            var latesVersion = await GetLatestPackageVersion(packageInfo, resolverStrategy).ConfigureAwait(false);
            var version = SemVer.Parse(latesVersion);

            var packageLocation = await DownloadPackage(packageInfo, version).ConfigureAwait(false);
            var dependencies = await GetAllDependencies(packageInfo, version).ConfigureAwait(false);

            var files = NuGetV2.GetLibFiles(packageLocation).Concat(dependencies);

            var package = new Package
            {
                Info = packageInfo,
                Version = latesVersion,
                Files = files.ToArray()
            };

            return package;
        }

        private async Task<string> GetLatestPackageVersion(PackageInfo packageInfo, ResolverStrategy strategy)
        {
            var requirement = VersionRequirement.Parse(packageInfo.VersionConstraint);
            var versions = await GetAllVersions(packageInfo.PackageName);
            var matchingVersions = versions.Where(v => requirement.IsInRange(v, FSharpOption<bool>.None)).ToList();
            return strategy.IsMax ? matchingVersions.Max().AsString : matchingVersions.Min().AsString;
        }

        private async Task<IEnumerable<FileInfo>> GetAllDependencies(PackageInfo package, SemVerInfo version)
        {
            var packageDetails = await GetPackageDetails(package.PackageName, version);

            var fileInfos = new List<FileInfo>();

            var dependencies = packageDetails.Dependencies.Select(d => new PackageInfo(d.Item1.ToString(), d.Item2.FormatInNuGetSyntax()));

            foreach (var dependency in dependencies)
            {
                var result = await DownloadPackageWithDependencies(dependency, ResolverStrategy.Min);

                fileInfos.AddRange(result.Files);
            }

            return fileInfos;
        }

        private async Task<string> DownloadPackage(PackageInfo package, SemVerInfo version)
        {
            var result = await FSharpAsync.StartAsTask(
                NuGetV2.DownloadPackage(packagesStore, 
                                        FSharpOption<Utils.Auth>.None, 
                                        nugetFeed, 
                                        Domain.GroupName("default"), 
                                        Domain.PackageName(package.PackageName), 
                                        version, 
                                        includeVersionInPath: true, 
                                        force: false),
                FSharpOption<TaskCreationOptions>.None,
                FSharpOption<CancellationToken>.None);
            return result;
        }

        private async Task<NuGetV2.NugetPackageCache> GetPackageDetails(string packageName, SemVerInfo version)
        {
            var result = await FSharpAsync.StartAsTask(
                NuGetV2.getDetailsFromNuGet(true, FSharpOption<Utils.Auth>.None, nugetFeed, Domain.PackageName(packageName), version),
                FSharpOption<TaskCreationOptions>.None,
                FSharpOption<CancellationToken>.None);

            return result;
        }

        private async Task<IEnumerable<SemVerInfo>> GetAllVersions(string packageName)
        {
            var result = await FSharpAsync.StartAsTask(
                NuGetV2.getAllVersions(FSharpOption<Utils.Auth>.None, nugetFeed, packageName),
                FSharpOption<TaskCreationOptions>.None,
                FSharpOption<CancellationToken>.None);
            return result.Value.Select(SemVer.Parse);
        }
    }
}