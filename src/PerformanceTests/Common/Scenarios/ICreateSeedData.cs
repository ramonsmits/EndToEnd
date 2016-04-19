namespace Common.Scenarios
{
    using System.Threading.Tasks;

    interface ICreateSeedData
    {
        //int SeedSize { get; }

        /// <summary>
        /// Sends or publishes a single message
        /// </summary>
        Task SendMessage(ISession session);
    }
}
