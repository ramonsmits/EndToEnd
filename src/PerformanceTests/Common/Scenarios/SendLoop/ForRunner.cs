using System;
using System.Threading.Tasks;

class ForRunner : SendLoop
{
    protected override async Task Batch(int count, Func<Task> action)
    {
        for (var i = 0; i < count; i++) await action().ConfigureAwait(false);
    }
}