﻿using System.Threading;
using System.Threading.Tasks;
using Common.Scenarios;
using NServiceBus;
using NServiceBus.Logging;

/// <summary>
/// Does a continuous test where a pre-seeded amount of messages will be handled
/// </summary>    
partial class ReceiveRunner : BaseRunner, ICreateSeedData
{
    static readonly ILog Log = LogManager.GetLogger(nameof(ReceiveRunner));
    static readonly AsyncCountdownEvent countdownEvent = new AsyncCountdownEvent(0);
    int seedCount;

    public class Command : ICommand
    {
        public byte[] Data { get; set; }
    }

    partial class Handler
    {
    }

    public Task SendMessage(ISession session)
    {
        Interlocked.Increment(ref seedCount);
        return session.SendLocal(new Command { Data = Data });
    }

    static void Signal()
    {
        countdownEvent.Signal();
    }

    protected override Task Wait()
    {
        return Task.WhenAny(base.Wait(), WaitForCountDown());
    }

    async Task WaitForCountDown()
    {
        await countdownEvent.WaitAsync().ConfigureAwait(false);
        Log.Info("All messages received!");
    }

    protected override Task Setup()
    {
        countdownEvent.Reset(seedCount);
        return Task.FromResult(0);
    }
}

