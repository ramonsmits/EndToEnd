using System;
using System.Linq;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Routing;
using Variables;

class SenderSideProfile : IProfile, INeedContext
{
    public IContext Context { private get; set; }

    public void Configure(EndpointConfiguration cfg)
    {
        if (Context.Permutation.ScaleOut != ScaleOut.SenderSide) return;
        if (Context.Permutation.Transport != Transport.MSMQ) throw new NotSupportedException("SenderSide should only be used with MSMQ");

        var senderSideArgs = ConfigurationHelper.FetchSetting("SenderSide");

        if (string.IsNullOrWhiteSpace(senderSideArgs)) throw new InvalidOperationException("Setting `SenderSide` not resolved.");

        var machines = senderSideArgs.Split('|');

        /*
        // Undocumented alternative
        var instances = machines.Select(x => new EndpointInstance(Context.EndpointName).AtMachine(x)).ToArray();
        cfg.GetSettings().GetOrCreate<EndpointInstances>().Add(instances);
        */

        cfg.RegisterComponents(x => x.RegisterSingleton(new StaticEndpointMapping { EndpointName = Context.EndpointName, Machines = machines }));
    }

    class StaticEndpointMapping : Feature
    {
        public string EndpointName;
        public string[] Machines;

        protected override void Setup(FeatureConfigurationContext context)
        {
            if (EndpointName == null || Machines == null) throw new InvalidOperationException("Fields not initialized.");

            var instances = Machines.Select(x => new EndpointInstance(EndpointName).AtMachine(x)).ToArray();

            var endpointInstances = context.EndpointInstances();
            endpointInstances.Add(instances);
        }
    }
}

