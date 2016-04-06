namespace Integration
{
    using System.Linq;
    using NUnit.Framework;
    using Tests.Permutations;
    using Tests.Tools;
    using Variables;

    [TestFixture]
    public class PermutationDirectoryResolverTests
    {
        PermutationDirectoryResolver resolver;

        [SetUp]
        public void Setup()
        {
            resolver = new PermutationDirectoryResolver(TestContext.CurrentContext.WorkDirectory);
        }

        [Test]
        public void Should_resolve_files_based_on_premutation()
        {
            var permutation = new Permutation
            {
                Persister = Persistence.NHibernate,
                Version = NServiceBusVersion.V6
            };


        var result = resolver.Resolve(permutation);
            Assert.That(result.RootProjectDirectory, Is.EqualTo("NServiceBus6"));
            Assert.That(result.Files, Is.Not.Empty);
        }

        [Test]
        public void Should_resolve_persisters(Persistence persister, NServiceBusVersion version, string fileName)
        {
            var permutation = new Permutation
            {
                Persister = persister,
                Transport = Transport.MSMQ,
                Version = version
            };

            var result = resolver.Resolve(permutation);
            Assert.That(result.Files.Any(info => info.Name == "Transport.V6.RabbitMQ_v4.dll"));
            Assert.That(result.Files.Any(info => info.Name == "Persistence.V6.NHibernate_v7.dll"));
        }

        [Test]
        public void Should_resolve_unversioned_dependencies()
        {
            var permutation = new Permutation
            {
                Persister = Persistence.InMemory,
                Transport = Transport.MSMQ,
                Version = NServiceBusVersion.V5
            };

            var result = resolver.Resolve(permutation);
            Assert.That(result.Files.Any(info => info.Name == "Transport.V5.MSMQ.dll"));
            Assert.That(result.Files.Any(info => info.Name == "Persistence.V5.InMemory.dll"));
        }

        [Test]
        public void Should_not_contain_NServiceBus_Core_assembly()
        {
            var permutation = new Permutation
            {
                Persister = Persistence.NHibernate,
                Version = NServiceBusVersion.V6
            };

            var result = resolver.Resolve(permutation);
            Assert.That(result.Files.All(info => info.Name != "NServiceBus.Core.dll"));
        }
    }
}