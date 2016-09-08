using System;
using System.Threading.Tasks;

public class TaskHelper
{
    public static async Task ParallelFor(int count, Func<int, Task> action)
    {
        var sends = new Task[count];
        for (var i = 0; i < count; i++) sends[i] = action(i);
        await Task.WhenAll(sends);
    }
}
