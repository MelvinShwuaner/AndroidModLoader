using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Linq;
using IEnumerable = Il2CppSystem.Collections.IEnumerable;
namespace NeoModLoader.AndroidCompatibilityModule;

public static partial class Extentions
{
    public static Il2CppSystem.Collections.Generic.List<T> ToList<T>(this Il2CppObjectBase Object) where T : Il2CppSystem.Object
    {
        var enumerable = Object.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.ToList();
    }
    public static IEnumerable AsEnumerable(this Il2CppObjectBase obj)
    {
        return obj.Cast<IEnumerable>();
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
	    return enumerable.OrderBy(Converter.C<Il2CppSystem.Func<T, K>>(func));
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