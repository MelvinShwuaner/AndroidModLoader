using Il2CppSystem.Collections;
using Sys = Il2CppSystem;
using Il2CppInterop.Runtime;
using NeoModLoader.AndroidCompatibilityModule;
/// <summary>
/// collection of tools to allow mods to work on il2cpp and mono on the same code
/// </summary>
public static partial class Extentions
{
    public static Sys.ValueTuple<X, Y, Z> C<X, Y, Z>(this ValueTuple<X, Y, Z> tuple)
    {
        return new Sys.ValueTuple<X, Y, Z>(tuple.Item1, tuple.Item2, tuple.Item3);
    }
    public static IEnumerator C(this global::System.Collections.IEnumerator enumerator)
    {
        return new IL2CPPEnumerator(enumerator).Cast<IEnumerator>();
    }

    public static IEnumerable<T> C<T>(this Sys.Collections.Generic.IEnumerable<T> c)
    {
        var enumerator = c.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }
    public static Sys.ValueTuple<X, Y> C<X, Y>(this ValueTuple<X, Y> tuple)
    {
        return new Sys.ValueTuple<X, Y>(tuple.Item1, tuple.Item2);
    }
    public static Sys.Type C (this Type type)
    {
        return Il2CppType.From(type);
    }
    public static Type C(this Sys.Type type)
    {
        var name = type.AssemblyQualifiedName;
        return Type.GetType(name) ?? Type.GetType("Il2Cpp"+name);
    }
    public static MonoException C(this Exception e)
    {
        return new MonoException(e);
    }
    public static Il2CppException C(this Sys.Exception e)
    {
        return new Il2CppException(e.Pointer);
    }
    public static Sys.Collections.Generic.HashSet<E> C<E>(this HashSet<E> set)
    {
        Sys.Collections.Generic.HashSet<E> hash = new();
        foreach (var VARIABLE in set)
        {
            hash.Add(VARIABLE);
        }
        return hash;
    }
    public static HashSet<E> C<E>(this Sys.Collections.Generic.HashSet<E> set)
    {
        HashSet<E> hash = new();
        foreach (var VARIABLE in set)
        {
            hash.Add(VARIABLE);
        }
        return hash;
    }
    public static Sys.Collections.Generic.List<E> C<E>(this List<E> e)
    {
        Sys.Collections.Generic.List<E> list = new();
        foreach (var item in e)
        {
            list.Add(item);
        }
        return list;
    }
    public static List<E> C<E>(this Sys.Collections.Generic.List<E> e)
    {
        List<E> list = new List<E>();
        foreach (var item in e)
        {
            list.Add(item);
        }
        return list;
    }
    public static Dictionary<key, value> C<key, value>(this Sys.Collections.Generic.Dictionary<key, value> e)
    {
        Dictionary<key, value> dictionary = new Dictionary<key, value>();
        foreach (var item in e)
        {
            dictionary.Add(item.Key, item.Value);
        }
        return dictionary;
    }
    public static Sys.Collections.Generic.Dictionary<key, value> C<key, value>(this Dictionary<key, value> e)
    {
        Sys.Collections.Generic.Dictionary<key, value> dictionary = new();
        foreach (var item in e)
        {
            dictionary.Add(item.Key, item.Value);
        }
        return dictionary;
    }
}