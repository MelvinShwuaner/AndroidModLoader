namespace NeoModLoader.utils.Collections;
/// <summary>
/// a node in a collection which has nodes before it, after it, and a priority
/// </summary>
public interface INode<T> where T : INode<T>
{
    /// <summary>
    /// Nodes which are before this
    /// </summary>
    public HashSet<T> BeforeThis {get;}
    /// <summary>
    /// nodes which are after this
    /// </summary>
    public HashSet<T> AfterThis { get; }
    /// <summary>
    /// the general Priority of the node. 0 for first in the collection
    /// </summary>
    public int Priority { get; }
}

public static class INodeExtentions
{
    /// <summary>
    /// returns true if <see cref="thisnode"/> is after <see cref="node"/>
    /// </summary>
    public static bool IsAfter<T>(this T thisnode, T node) where T  : INode<T>
    {
        return thisnode.BeforeThis.Contains(node) || thisnode.BeforeThis.Any(before => before.IsAfter(node));
    }
    /// <summary>
    /// returns true if <see cref="thisnode"/> is before <see cref="node"/>
    /// </summary>
    public static bool IsBefore<T>(this T thisnode, T node) where T  : INode<T>
    {
        return thisnode.AfterThis.Contains(node) || thisnode.AfterThis.Any(after => after.IsBefore(node));
    }
}