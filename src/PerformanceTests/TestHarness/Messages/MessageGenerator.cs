using Messages.Incoming;
using System;
using System.Collections.Generic;
using System.Linq;

public static class MessageGenerator
{
    static string[] StockCodes = new[]
    {
        "APL",
        "MSFT",
        "STX40"
    };

    private static Random RandomGenerator = new Random(0);

    public static IEnumerable<object> GenerateMessages(int numberOfSmallMessages, int numberOfSmallFailures, int numberOfLargeMessages, int numberOfLargeFailures)
    {
        var largeMessages = Enumerable.Range(0, numberOfLargeMessages).Select(i => new SensAnnouncement
        {
            StockCode = StockCodes[RandomGenerator.Next(StockCodes.Length)],
            Time = DateTime.UtcNow,
            Header = StringGenerator.GenerateRandomString(40),
            Body = StringGenerator.GenerateRandomString(100000),
            Id = Guid.NewGuid(),
        });

        var largeFailures = Enumerable.Range(0, numberOfLargeFailures).Select(i => new SensAnnouncement
        {
            StockCode = StockCodes[RandomGenerator.Next(StockCodes.Length)],
            Time = DateTime.UtcNow,
            Header = StringGenerator.GenerateRandomString(40),
            Body = StringGenerator.GenerateRandomString(100000),
            Id = Guid.NewGuid(),
            ShouldMessageFail = true,
        });

        var smallMessages = Enumerable.Range(0, numberOfSmallMessages).Select(i => new TradeMatched
        {
            StockCode = StockCodes[RandomGenerator.Next(StockCodes.Length)],
            Time = DateTime.UtcNow,
            TradePrice = (decimal) RandomGenerator.NextDouble()*100.0m,
            TradeVolume = RandomGenerator.Next(),
            Id = Guid.NewGuid(),
        });

        var smallFailures = Enumerable.Range(0, numberOfSmallFailures).Select(i => new TradeMatched
        {
            StockCode = StockCodes[RandomGenerator.Next(StockCodes.Length)],
            Time = DateTime.UtcNow,
            TradePrice = 10,
            TradeVolume = 123142312,
            Id = Guid.NewGuid(),
            ShouldMessageFail = true,
        });

        return largeMessages.Cast<object>()
            .Union(smallMessages)
            .Union(largeFailures)
            .Union(smallFailures)
            .OrderBy(msg => RandomGenerator.Next());
    }
}