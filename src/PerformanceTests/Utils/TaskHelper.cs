using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TaskHelper
{
    static readonly int cores = Environment.ProcessorCount;

    public static async Task ParallelFor(int count, Func<Task> action)
    {
        // If we have a batch of 10,000 and have 10 cores we do 10 iterations of 1,000 message chunks.
        for (var i = 0; i < cores; i++)
        {
            var chunkSize = count / cores;
            var sends = new List<Task>(chunkSize);
            for (var j = 0; j < chunkSize; j++)
            {
                sends.Add(action());
            }
            await Task.WhenAll(sends);
        }
    }

}

