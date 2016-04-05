using Categories;
using NServiceBus;
using Variables;

class TransactionProfile : IProfile, INeedPermutation
{
    public Permutation Permutation { private get; set; }

    public void Configure(BusConfiguration busConfiguration)
    {
        if (Permutation.DTCMode == DTC.Off)
            busConfiguration.Transactions().DisableDistributedTransactions();
    }
}
