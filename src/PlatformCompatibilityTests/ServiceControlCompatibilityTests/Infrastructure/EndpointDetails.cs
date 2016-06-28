namespace ServiceControlCompatibilityTests
{
    using System;
    using System.Collections.Generic;
    using Autofac;

    interface IEndpointDetails
    {
        IContainer CreateContainer();
        string Name { get; }
    }

    class EndpointDetails : IEndpointDetails
    {
        IList<Action<ContainerBuilder>> additions = new List<Action<ContainerBuilder>>();

        public EndpointDetails(string name)
        {
            Name = name;
        }

        public EndpointDetails With<T>(T instance) where T : class
        {
            additions.Add(builder => builder.RegisterInstance(instance));
            return this;
        }

        public EndpointDetails With<T>()
        {
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

        public string Name { get; }
    }
}
