using System;
using System.Threading.Tasks;

public class TaskBatch : IBatchHelper
{
    public Task Batch(int count, Func<int, Task> action)
    {
        var sends = new Task[count];
        for (var i = 0; i < count; i++) sends[i] = action(i);
        return Task.WhenAll(sends);
    }
}


public class ParallelForBatch : IBatchHelper
{
    static ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

    public Task Batch(int count, Func<int, Task> action)
    {
        return Task.Run(() => Parallel.For(0, count, po, i => action(i).GetAwaiter().GetResult()));
    }
}

public static class BatchHelper
{
    public static IBatchHelper Instance;
}

public interface IBatchHelper
{
    Task Batch(int count, Func<int, Task> action);
}
