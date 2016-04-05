namespace Integration
{
    using System.Linq;
    using Categories;
    using NUnit.Framework;
    using Tests.Tools;
    using Variables;

    [TestFixture]
    public class PermutationDirectoryResolverTests
    {
        [Test]
        public void Should_resolve_files_based_on_premutation()
        {
            var resolver = new PermutationDirectoryResolver(TestContext.CurrentContext.WorkDirectory);

            var permutation = new Permutation
            {
                Persister = Persistence.NHibernate,
                Version = NServiceBusVersion.V6
            };

            var result = resolver.Resolve(permutation).ToList();
            Assert.That(result.Count > 0);
            Assert.That(result[0].ComponentName, Is.EqualTo("Persistence.V6.NHibernate"));
            Assert.That(result[0].Files, Is.Not.Empty);
        }

        [Test]
        public void Should_not_contain_NServiceBus_Core_assembly()
        {
            var resolver = new PermutationDirectoryResolver(TestContext.CurrentContext.WorkDirectory);

            var permutation = new Permutation
            {
                Persister = Persistence.NHibernate,
                Version = NServiceBusVersion.V6
            };

            var result = resolver.Resolve(permutation).First();
            Assert.That(result.Files.All(info => info.Name != "NServiceBus.Core.dll"));
        }
    }
}