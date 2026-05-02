namespace NeoModLoader.utils.Collections;
/// <summary>
/// a simple implementation of a concurrent IList, it is a List by default
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentList<T> : ConcurrentCollection<T>, IList<T>, IReadOnlyList<T>
{
    public readonly IList<T> List;
    public ConcurrentList(IList<T> collection) : base(collection)
    {
        List = collection;
    }
    public ConcurrentList() : this(new List<T>()){}

    public int IndexOf(T item)
    {
        lock (_lock)
        {
            return List.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        lock (_lock)
        {
           List.Insert(index, item);
        }
    }

    public void RemoveAt(int index)
    {
        lock (_lock)
        {
         List.RemoveAt(index);
        }
    }

    public T this[int index]
    {
        get
        {
            lock (_lock)
            {
                return List[index];
            }
        }
        set
        {
            lock (_lock)
            {
                 List[index] = value;
            }
        }
    }
}