namespace NeoModLoader.utils.Collections;
/// <summary>
/// a simple implementation of thread safe ISet, it is a hashset by default
/// </summary>
public class ConcurrentSet<T> : ConcurrentCollection<T>, ISet<T>
{
    public readonly ISet<T> Set;
    public ConcurrentSet(ISet<T> collection) : base(collection)
    {
        Set = collection;
    }
    public ConcurrentSet() : this(new HashSet<T>()){}

    public bool Add(T item)
    {
        lock (_lock)
        {
            return Set.Add(item);
        }
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<T> other)
    {
        lock (_lock)
        {
            Set.UnionWith(other);
        }
    }
    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<T> other)
    {
        lock (_lock)
        {
            Set.IntersectWith(other);
        }
    }
    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<T> other)
    {
        lock (_lock)
        {
            Set.ExceptWith(other);
        }
    }
    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        lock (_lock)
        {
            Set.SymmetricExceptWith(other);
        }
    }
    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other)
    {
        lock (_lock)
        {
            return Set.IsSubsetOf(other);
        }
    }
    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other)
    {
        lock (_lock)
        {
            return Set.IsSupersetOf(other);
        }
    }
    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        lock (_lock)
        {
            return Set.IsProperSupersetOf(other);
        }
    }
    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        lock (_lock)
        {
            return Set.IsProperSubsetOf(other);
        }
    }
    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other)
    {
        lock (_lock)
        {
            return Set.Overlaps(other);
        }
    }
    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other)
    {
        lock (_lock)
        {
            return Set.SetEquals(other);
        }
    }
}