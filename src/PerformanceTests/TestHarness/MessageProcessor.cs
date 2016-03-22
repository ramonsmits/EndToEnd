using Messages.Incoming;
using Messages.Notifications;
using Messages.Outgoing;
using Messages.TradeActions;
using NServiceBus;
using System;

public class MessageProcessor : IHandleMessages<SensAnnouncement>, IHandleMessages<TradeMatched>
{
    IBus bus;
    ISimulateUserDataAccess dataAccessSimulation;


    public MessageProcessor(IBus bus, ISimulateUserDataAccess dtcEscalation)
    {
        this.bus = bus;
        dataAccessSimulation = dtcEscalation;
    }

    private static readonly ObjectPool<Random> pool = new ObjectPool<Random>(i => new Random(i));

    public void Handle(TradeMatched message)
    {
        try
        {
            dataAccessSimulation.DoSomething();

            if (message.ShouldMessageFail)
            {
                throw new ApplicationException("Faking an exception during message processing...");
            }

            var shouldTrade = ShouldExecuteTrade(5);
            if (shouldTrade == 1)
            {
                var orderMessage = new PlaceLongTrade
                {
                    Id = Guid.NewGuid(),
                    StockCode = message.StockCode,
                    OfferPrice = message.TradePrice*1.02m,
                    Volume = (int) Math.Floor(message.TradeVolume*0.5)
                };
                bus.Send(orderMessage);

                bus.Publish(new TradePlaced
                {
                    StockCode = orderMessage.StockCode,
                    Type = "Long",
                    Volume = orderMessage.Volume
                });

                return;
            }

            if (shouldTrade == -1)
            {
                var orderMessage = new PlaceShortTrade
                {
                    Id = Guid.NewGuid(),
                    StockCode = message.StockCode,
                    AskingPrice = message.TradePrice*0.98m,
                    Volume = (int) Math.Floor(message.TradeVolume*0.5)
                };
                bus.Send(orderMessage);

                bus.Publish(new TradePlaced
                {
                    StockCode = orderMessage.StockCode,
                    Type = "Short",
                    Volume = orderMessage.Volume
                });

                return;
            }

            if (shouldTrade == 0)
            {
                bus.Reply(new QueryTrade
                {
                    TradeId = message.Id
                });
            }
        }
        finally
        {
            MetricReaper.Meter.Mark();
            MessageCounter.Decrement();
        }
    }

    public void Handle(SensAnnouncement message)
    {
        try
        {
            dataAccessSimulation.DoSomething();

            if (message.ShouldMessageFail)
            {
                throw new ApplicationException("Faking an exception during message processing...");
            }

            bus.Publish(new SensAnnouncementReceived
            {
                StockCode = message.StockCode,
                Header = message.Header,
                Body = message.Body
            });
        }
        finally
        {
            MetricReaper.Meter.Mark();
            MessageCounter.Decrement();
        }
    }

    private int ShouldExecuteTrade(double percentageChanceOfTrade)
    {
        var random = pool.GetObject();
        try
        {
            if (random.Next(100) + 1 <= percentageChanceOfTrade)
            {
                // -1 for Short, 1 for Long, 0 reply message to sender
                return random.Next(3) - 1;
            }

            return int.MaxValue;
        }
        finally
        {
            pool.PutObject(random);
        }
    }
}