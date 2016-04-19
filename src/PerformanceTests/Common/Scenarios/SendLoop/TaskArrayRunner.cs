using System;
using System.Threading.Tasks;

class TaskArrayRunner : SendLoop
{
    protected override Task Batch(int count, Func<Task> action)
    {
        var sends = new Task[count];
        for (var i = 0; i < count; i++) sends[i] = action();
        return Task.WhenAll(sends);
    }
}