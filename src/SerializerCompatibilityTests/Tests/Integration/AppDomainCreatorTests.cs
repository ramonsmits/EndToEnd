namespace Tests.Integration
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Common.Runner.AppDomains;
    using Common.Runner.Nuget;
    using NUnit.Framework;

    [TestFixture]
    public class AppDomainCreatorTests
    {
        [SetUp]
        public void Setup()
        {
            nuGetPackageResolver = new NuGetPackageResolver("../..");
            domainCreator = new AppDomainCreator();
        }

        AppDomain CreateDomain(PackageInfo packageInfo)
        {
            var package = nuGetPackageResolver.DownloadPackageWithDependencies(packageInfo).Result;
            var appDomainDescriptor = domainCreator.CreateDomain(TestsGlobal.BinDirectoryTemplate, package);
            return appDomainDescriptor.AppDomain;
        }

        [Test]
        public void Can_setup_an_appdomain()
        {
            Assert.That(() => CreateDomain("NServiceBus:5.2.5"), Throws.Nothing);
        }

        [Test]
        public void Should_run_in_a_different_appdomain()
        {
            var domain = CreateDomain("NServiceBus:5.2.5");

            Assert.That(domain, Is.Not.EqualTo(AppDomain.CurrentDomain));
        }

        [Test]
        public void Each_appdomain_should_have_a_different_version_of_nservicebus()
        {
            var firstDomain = CreateDomain("NServiceBus:[5.2.5]");
            var secondDomain = CreateDomain("NServiceBus:[5.0.5]");

            var nsb525 = Directory.GetFiles(firstDomain.BaseDirectory, "NServiceBus.Core.dll").First();
            var nsb505 = Directory.GetFiles(secondDomain.BaseDirectory, "NServiceBus.Core.dll").First();

            Assert.That(FileVersionInfo.GetVersionInfo(nsb525).FileVersion, Is.EqualTo("5.2.5.0"));
            Assert.That(FileVersionInfo.GetVersionInfo(nsb505).FileVersion, Is.EqualTo("5.0.5.0"));
        }

        [TestCase("NServiceBus:[5.2.5]", "NServiceBus 5.2.5")]
        [TestCase("NServiceBus:[5.0.5]", "NServiceBus 5.0.5")]
        public void Run_task_inside_correct_appdomain(string package, string domainName)
        {
            var domain = CreateDomain(package);

            Assert.That(domain.GetData("FriendlyName"), Is.EqualTo(domainName));
        }

        AppDomainCreator domainCreator;
        NuGetPackageResolver nuGetPackageResolver;
    }
}