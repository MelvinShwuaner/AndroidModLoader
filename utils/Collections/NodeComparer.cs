namespace NeoModLoader.utils.Collections;

public class NodeComparer<T> : IComparer<T>, ISorter<T> where T : INode<T>
{
    /// <summary>
    /// Sorts a collection of Nodes based off their properties
    /// </summary>
    public static List<T> Sort(IEnumerable<T> pNodes)
    {
        HashSet<T> selected_nodes = new(pNodes);
        Dictionary<T, int> node_in_degree = new();
        Queue<T> queue = new();
        foreach (T node in selected_nodes.OrderBy(node => node.Priority))
        {
            int in_degree = node.BeforeThis.Count(before => selected_nodes.Contains(before));
            node_in_degree.Add(node, in_degree);
            if (in_degree == 0)
            {
                queue.Enqueue(node);
            }
        }
        List<T> nodes = new();
        while (queue.Count > 0)
        {
            T curr_node = queue.Dequeue();
            nodes.Add(curr_node);

            foreach (T after in curr_node.AfterThis.OrderBy(node => node.Priority))
            {
                if (!selected_nodes.Contains(after))
                {
                    continue;
                }

                if (!node_in_degree.ContainsKey(after))
                {
                    continue;
                }

                node_in_degree[after]--;
                if (node_in_degree[after] == 0)
                {
                    queue.Enqueue(after);
                }
            }
        }
        if (nodes.Count == selected_nodes.Count)
        {
            return nodes;
        }
        foreach (T remaining_node in selected_nodes
                     .Where(node => !nodes.Contains(node))
                     .OrderBy(node => node.Priority))
        {
            nodes.Add(remaining_node);
        }
        return nodes;
    }
    public int Compare(T x, T y)
    {
        if (x.IsBefore(y) || y.IsAfter(x))
        {
            return -1;
        }
        if (y.IsBefore(x) || x.IsAfter(y))
        {
            return 1;
        }
        return x.Priority.CompareTo(y.Priority);
    }

    IEnumerable<T> ISorter<T>.Sort(IEnumerable<T> collection)
    {
       return Sort(collection);
    }
}