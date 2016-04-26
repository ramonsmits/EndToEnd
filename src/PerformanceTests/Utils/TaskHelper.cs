using System;
using System.Threading.Tasks;

public class TaskHelper
{
    public static async Task ParallelFor(int count, Func<int, Task> action)
    {
        await Task.Yield();
        Parallel.For(0, count, i => action(i).GetAwaiter().GetResult());
    }
}
