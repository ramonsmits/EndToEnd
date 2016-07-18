namespace ServiceControlCompatibilityTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;

    interface IEndpointDetails
    {
        IContainer CreateContainer();
        string Name { get; }
        IEnumerable<Type> GetTypes();
    }

    class EndpointDetails : IEndpointDetails
    {
        IList<Action<ContainerBuilder>> additions = new List<Action<ContainerBuilder>>();
        HashSet<Type> types = new HashSet<Type>();

        public EndpointDetails(string name)
        {
            Name = name;
        }

        public EndpointDetails With<T>(T instance) where T : class
        {
            types.Add(typeof(T));
            additions.Add(builder => builder.RegisterInstance(instance));
            return this;
        }

        public EndpointDetails With<T>()
        {
            types.Add(typeof(T));
            additions.Add(builder => builder.RegisterType<T>());
            return this;
        }

        IContainer IEndpointDetails.CreateContainer()
        {
            var builder = new ContainerBuilder();

            foreach (var addition in additions)
            {
                addition(builder);
            }

            return builder.Build();
        }

        public IEnumerable<Type> GetTypes() => types.AsEnumerable();

        public string Name { get; }
    }
}
