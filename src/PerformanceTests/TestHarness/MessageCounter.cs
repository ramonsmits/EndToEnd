using System.Threading;

public class MessageCounter
{
    static int currentCount;
    static IHandleCompleted completedWatcher;

    public static void SubscribeToCompleted(IHandleCompleted instance)
    {
        completedWatcher = instance;
    }

    public static void Decrement()
    {
        var result = Interlocked.Decrement(ref currentCount);

        if (result == 0)
        {
            completedWatcher?.Completed();
        }
    }

    public static void Increment()
    {
        Interlocked.Increment(ref currentCount);
    }

    internal static int Current()
    {
        return currentCount;
    }
}
