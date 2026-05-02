using System.Collections;
using System.Collections.ObjectModel;

namespace NeoModLoader.utils.Collections;
/// <summary>
/// A simple implementation of a concurrent collection, it is a Collection by default
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentCollection<T> : ICollection<T>
{
    public readonly ICollection<T> Collection;
    protected readonly object _lock = new();

    public ConcurrentCollection(ICollection<T> collection)
    {
        Collection = collection;
    }
    public ConcurrentCollection() : this(new Collection<T>()) { }
    public IEnumerator<T> GetEnumerator()
    {
        List<T> list;
        lock (_lock)
        {
            list = Collection.ToList();
        }
        return list.GetEnumerator();    
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        lock (_lock)
        {
            Collection.Add(item);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            Collection.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return Collection.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_lock)
        {
            Collection.CopyTo(array, arrayIndex);
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
           return Collection.Remove(item);
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return Collection.Count;
            }
        }
    }

    public bool IsReadOnly { get
    {
        lock (_lock)
        {
            return Collection.IsReadOnly;
        }
    } }
}