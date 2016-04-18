using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

abstract class LoopRunner : BaseRunner
{
    ILog Log = LogManager.GetLogger(typeof(LoopRunner));

    Task loopTask;

    static CountdownEvent countdownEvent { get; set; }

    CancellationTokenSource stopLoop { get; set; }
    bool Shutdown { get; set; }
    protected abstract int BatchSize { get; set; }
    protected abstract Task SendMessage();

    protected override void Start()
    {
        if (BatchSize == default(int))
            throw new InvalidOperationException("BatchSize property has not been set.");

        stopLoop = new CancellationTokenSource();
        loopTask = Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
    }

    protected override void Stop()
    {
        Shutdown = true;
        using (stopLoop)
        {
            stopLoop.Cancel();
            using (loopTask)
            {
                loopTask.Wait();
            }
        }
    }

    public class Handler : IHandleMessages<IMessage>
    {
        ILog Log = LogManager.GetLogger(typeof(Handler));

#if Version5
        public void Handle(IMessage message)
        {
            Log.Info($"This is {this.GetType()}");
            LoopRunner.countdownEvent.Signal();
        }
#else
        public async Task Handle(IMessage message, IMessageHandlerContext context)
        {
            LoopRunner.countdownEvent.Signal();
        }
#endif
    }

    async Task Loop(object o)
    {
        try
        {
            countdownEvent = new CountdownEvent(BatchSize);

            Log.Warn("Sleeping for the bus to purge the queue. Loop requires the queue to be empty.");
            Thread.Sleep(5000);
            Log.Info("Starting");

            while (!Shutdown)
            {
                try
                {
                    Console.Write("1");
                    countdownEvent.Reset(BatchSize);

                    var d = Stopwatch.StartNew();

                    for (var i = 0; i < Environment.ProcessorCount; i++)
                    {
                        var sends = new List<Task>();
                        for (var j = 0; j < countdownEvent.InitialCount / Environment.ProcessorCount; j++)
                        {
                            sends.Add(SendMessage());
                        }
                        await Task.WhenAll(sends);
                    }

                    if (d.Elapsed < TimeSpan.FromSeconds(2.5))
                    {
                        BatchSize *= 2;
                        Log.InfoFormat("Batch size increased to {0}", BatchSize);
                    }

                    Console.Write("2");

                    countdownEvent.Wait(stopLoop.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            Log.Info("Stopped");
        }
        catch (Exception ex)
        {
            Log.Error("Loop", ex);
        }
    }
}
