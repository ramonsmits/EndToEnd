namespace ServiceControlCompatibilityTests
{
    using System.Configuration;

    public interface ITransportDetails
    {
        string TransportName { get; }
        void ApplyTo(Configuration configuration);
    }
}