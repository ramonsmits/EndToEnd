using System;
using System.Collections.Concurrent;
using System.Threading;

public class ObjectPool<T>
{
    readonly ConcurrentBag<T> objects;
    readonly Func<int, T> objectGenerator;
    int items;

    public ObjectPool(Func<int, T> objectGenerator)
    {
        objects = new ConcurrentBag<T>();
        this.objectGenerator = objectGenerator;
    }

    public T GetObject()
    {
        T item;
        if (objects.TryTake(out item)) return item;
        return objectGenerator(Interlocked.Increment(ref items));
    }

    public void PutObject(T item)
    {
        objects.Add(item);
    }
}
