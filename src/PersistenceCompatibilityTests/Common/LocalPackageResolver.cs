using System.IO;
using System.Linq;

namespace Common
{
    public class LocalPackageResolver
    {
        string localStore;

        public LocalPackageResolver(string localStore)
        {
            this.localStore = localStore;
        }

        public Package GetLocalPackage(PackageInfo packageInfo)
        {
            var versionFolder = Path.Combine(localStore, packageInfo.GetProjectTemplateVersion());
            var files = new DirectoryInfo(versionFolder).GetFiles();

            return new Package
            {
                Info = packageInfo,
                Version = packageInfo.Version,
                Files = files.ToArray()
            };
        }
    }
}