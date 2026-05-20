using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Collections;
using Il2CppSystem.Linq;
using IEnumerable = Il2CppSystem.Collections.IEnumerable;
using NeoModLoader.AndroidCompatibilityModule;

public static partial class Extentions
{
    public static Il2CppSystem.Collections.Generic.List<T> ToList<T>(this Il2CppObjectBase Object)
    {
        var enumerable = Object.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.ToList();
    }
    public static Il2CppSystem.Collections.Generic.List<T> AsList<T>(this IEnumerable<T> Object)
    {
        Il2CppSystem.Collections.Generic.List<T> list = new();
        foreach (var VARIABLE in Object)
        {
            list.Add(VARIABLE);
        }
        return list;
    }
    public static void UnionWith<T>(this HashSet<T> set, Il2CppSystem.Collections.Generic.IEnumerable<T> other)
    {
        set.UnionWith(other.C());
    }
    public static Il2CppSystem.Nullable<A> N<A>(this A a) where A : new()
    {
        return new Il2CppSystem.Nullable<A>(a);
    }
    public static bool MoveNext<T>(this Il2CppSystem.Collections.Generic.IEnumerator<T> enumerator)
    {
        return enumerator.Cast<IEnumerator>().MoveNext();
    }
    public static T[] AddToArray<T>(this Il2CppArrayBase<T> sequence, T item)
    {
        return sequence.AddItem(item).ToArray();
    }
    public static IEnumerable AsEnumerable(this Il2CppObjectBase obj)
    {
        return obj.Cast<IEnumerable>();
    }
    public static Il2CppSystem.Collections.Generic.IEnumerable<T> AsEnumerable<T>(this Il2CppObjectBase obj)
    {
        return obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
    }
    public static T FirstOrDefault<T>(this Il2CppObjectBase obj)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.FirstOrDefault();
    }
    public static T FirstOrDefault<T>(this Il2CppObjectBase obj, Func<T, bool> predicate)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.FirstOrDefault((Il2CppSystem.Func<T, bool>)predicate);
    }
    public static Il2CppSystem.Collections.Generic.IEnumerable<T> Where<T>(this Il2CppObjectBase obj, Func<T, bool> func)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.Where((Il2CppSystem.Func<T, bool>)func);
    }
    public static Il2CppSystem.Collections.Generic.IEnumerable<T> Take<T>(this Il2CppObjectBase obj, int num)
    {
	    var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
	    return enumerable.Take(num);
    }
    public static Il2CppSystem.Linq.IOrderedEnumerable<T> OrderBy<T, K>(this Il2CppObjectBase obj, Func<T, K> func)
    {
	    var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
	    return enumerable.OrderBy(IL2CPPHelper.C<Il2CppSystem.Func<T, K>>(func));
    }
    public static Il2CppSystem.Collections.Generic.IEnumerable<R> Select<T, R>(this Il2CppObjectBase obj, Func<T, R> func)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.Select((Il2CppSystem.Func<T, R>)func);
    }
    public static bool Any<T>(this Il2CppObjectBase obj, Func<T, bool> func)
    {
	    var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
	    return enumerable.Any((Il2CppSystem.Func<T, bool>)func);
    }
}