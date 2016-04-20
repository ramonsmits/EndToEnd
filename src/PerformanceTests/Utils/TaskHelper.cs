using System;
using System.Threading.Tasks;

public class TaskHelper
{
    public static async Task ParallelFor(int count, Func<Task> action)
    {
        var sends = new Task[count];
        for (var i = 0; i < count; i++) sends[i] = action();
        await Task.WhenAll(sends);
    }
}
