namespace Common.Scenarios
{
    using NServiceBus;

    interface ICreateSeedData
    {
        int SeedSize { get; set; }

#if Version5
        /// <summary>
        /// Sends or publishes a single message
        /// </summary>
        void SendMessage(ISendOnlyBus sendOnlyBus);
#else
        /// <summary>
        /// Sends or publishes a single message
        /// </summary>
        System.Threading.Tasks.Task SendMessage(IEndpointInstance endpointInstance);
#endif
    }
}
