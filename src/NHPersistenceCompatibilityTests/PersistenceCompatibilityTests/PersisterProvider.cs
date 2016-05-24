using System;
using System.Collections.Generic;
using System.IO;
using Common;

namespace PersistenceCompatibilityTests
{
    public class PersisterProvider
    {
        public void Initialize(string[] nHibernatePackageVersions)
        {
            appDomainDescriptors = new List<AppDomainDescriptor>();
            cachedPersisterFacades = new Dictionary<string, PersisterFacade>();

            foreach (var version in nHibernatePackageVersions)
            {
                var appDomain = CreateAppDomain(version);

                appDomainDescriptors.Add(appDomain);


                var runner = new AppDomainRunner<IRawPersister>(appDomain);
                var facade = new PersisterFacade(runner);

                cachedPersisterFacades.Add(version, facade);
            }
        }

        AppDomainDescriptor CreateAppDomain(string versionName)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var packageResolver = new LocalPackageResolver(path);
            var domainCreator = new AppDomainCreator();

            var packageInfo = new PackageInfo("NServiceBus.NHibernate.Tests", versionName);
            var package = packageResolver.GetLocalPackage(packageInfo);
            var appDomainDescriptor = domainCreator.CreateDomain(package);
           
            return appDomainDescriptor;
        }

        public void Dispose()
        {
            foreach (var appDomainDescriptor in appDomainDescriptors)
            {
                appDomainDescriptor.Dispose();

                new FileInfo(appDomainDescriptor.ProjectAssemblyPath)
                    .Directory
                    ?.Delete(true);
            }
        }

        public PersisterFacade Get(string version)
        {
            return cachedPersisterFacades[version];
        }

        Dictionary<string, PersisterFacade> cachedPersisterFacades;
        IList<AppDomainDescriptor> appDomainDescriptors;
    }
}