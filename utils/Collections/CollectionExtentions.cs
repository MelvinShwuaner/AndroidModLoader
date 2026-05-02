
namespace NeoModLoader.utils.Collections;
/// <summary>
/// interface for sorting lists in your own way.
/// </summary>
public interface ISorter<T>
{
    public IEnumerable<T> Sort(IEnumerable<T> collection);
}
/// <summary>
/// very simple general sorter
/// </summary>
public class Sorter<T> : ISorter<T>
{
    public IEnumerable<T> Sort(IEnumerable<T> collection)
    {
        List<T> list = collection.ToList();
        list.Sort(comparer);
        return list;
    }
    private IComparer<T> comparer;
    public Sorter(IComparer<T> com = null)
    {
        comparer = com ?? Comparer<T>.Default;
    }
}

public static class CollectionExtentions
{
    public static IEnumerable<T> Sort<T>(this IEnumerable<T> collection, ISorter<T> sorter)
    {
        return sorter.Sort(collection);
    }
}