using System;
using System.Collections.Generic;

/// A super-generic object pool.
public class ObjectPool<T>
{
    private readonly Func<T> create;
    private readonly Action<T> reset;

    private readonly Queue<T> cache = new Queue<T>();

    public ObjectPool(Func<T> create, Action<T> reset)
    {
        this.create = create;
        this.reset = reset;
    }

    public T GetObject()
    {
        return cache.Count > 0 ? cache.Dequeue() : create();
    }

    public void ReleaseObject(T obj)
    {
        reset(obj);
        cache.Enqueue(obj);
    }
}
