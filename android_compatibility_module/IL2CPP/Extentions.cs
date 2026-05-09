using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Linq;
using RSG;
using UnityEngine;
using UnityEngine.Events;
using Enumerable = Il2CppSystem.Linq.Enumerable;
using IEnumerable = Il2CppSystem.Collections.IEnumerable;
using IEnumerator = Il2CppSystem.Collections.IEnumerator;
using Object = Il2CppSystem.Object;
using Random = System.Random;

namespace NeoModLoader.AndroidCompatibilityModule;
public static class Extentions
{
	public static void doUnits(this WorldTile tile, Action<Actor> action)
	{
		tile.doUnits(action);
	}
    public static bool IsValid(this Il2CppArrayBase arr)
    {
        return arr is { Length: > 0 };
    }
    //il2cpp array's indexof() is not good, better to just check pointers
    public static int GetIndex<T>(this Il2CppReferenceArray<T> arr, T obj) where T : Il2CppObjectBase
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].Pointer == obj.Pointer)
            {
                return i;
            }
        }

        return -1;
    }

    public static Il2CppSystem.Collections.Generic.HashSet<A> Get<A, B>(this SimSystemManager<A, B> manager) where A : BaseSimObject, new() where B : BaseObjectData, new()
    {
	    return manager._container._hashSet;
    }

    public static nint Clone(this GUIStyle orig)
    {
	    GUIStyle style = new GUIStyle(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GUIStyle>.NativeClassPtr));
	    style.m_Ptr = GUIStyle.Internal_Copy(orig, style);
	    return style.Pointer;
    }

    public static void addGenome(this ActorAsset asset, params ValueTuple<string, float>[] pListGenomePartsIDs)
    {
	    Il2CppSystem.ValueTuple<string, float>[] arr = new Il2CppSystem.ValueTuple<string, float>[pListGenomePartsIDs.Length];
	    for (var i = 0; i < pListGenomePartsIDs.Length; i++)
	    {
		    arr[i] = pListGenomePartsIDs[i].C();
	    }
	    asset.addGenome(arr);
    }
    public static IEnumerable<T> OfType<T>(this Il2CppObjectBase list) where T : Il2CppObjectBase
    {
	    foreach (var obj in list.Cast<IEnumerable>())
	    {
		    var cast = obj.TryCast<T>();
		    if (cast != null)
		    {
			    yield return cast;
		    }
	    }
    }
    /// <summary>
    /// is the type il2cpp compatible
    /// </summary>
    public static bool IsIL2CPPCompatible(this Type type)
    {
	    if (typeof(Il2CppSystem.Object).IsAssignableFrom(type))
		    return true;
	    return type.IsPrimitive || type == typeof(string);
    }
    #region Il2CppIEnumerable
    //extentions for ienumerables. since ienumerable isnt an interface in il2cpp we have to create extentions manually
    public static Il2CppSystem.Collections.Generic.List<T> ToList<T>(this Il2CppObjectBase Object) where T : Il2CppSystem.Object
    {
        var enumerable = Object.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.ToList();
    }
    public static T FirstOrDefault<T>(this Il2CppObjectBase obj)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.FirstOrDefault();
    }
    public static T FirstOrDefault<T>(this Il2CppObjectBase obj, Func<T, bool> predicate)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.FirstOrDefault(Converter.C<Il2CppSystem.Func<T, bool>>(predicate));
    }
    public static Il2CppSystem.Collections.Generic.IEnumerable<T> Where<T>(this Il2CppObjectBase obj, Func<T, bool> func)
    {
        var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
        return enumerable.Where(Converter.C<Il2CppSystem.Func<T, bool>>(func));
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
        return enumerable.Select(Converter.C<Il2CppSystem.Func<T, R>>(func));
    }
    public static bool Any<T>(this Il2CppObjectBase obj, Func<T, bool> func)
    {
	    var enumerable = obj.Cast<Il2CppSystem.Collections.Generic.IEnumerable<T>>();
	    return enumerable.Any(Converter.C<Il2CppSystem.Func<T, bool>>(func));
    }
    #endregion
    public static void setHoverAction(this TipButton button, Action action)
    {
	    button.hoverAction = action;
    }

    public static void setToggleAction(this GodPower button, Action<string> action)
    {
	    button.toggle_action = action;
    }
    //functions like listextention are useless to us now
    #region  Lists
    private static Random rnd => new();
	public static string ToJson(this List<string> list)
	{
		if (list.Count == 0)
		{
			return "[]";
		}
		return "['" + string.Join("','", list) + "']";
	}

	public static void ShuffleHalf<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			int count = list.Count;
			int num = count / 2 + 1;
			for (int i = 0; i < num && i < count; i += 2)
			{
				list.Swap(i, rnd.Next(i, count));
			}
		}
	}

	public static void ShuffleN<T>(this List<T> list, int pItems)
	{
		if (list.Count >= 2)
		{
			int num = ((list.Count < pItems) ? list.Count : pItems);
			for (int i = 0; i < num; i++)
			{
				list.Swap(i, rnd.Next(i, num));
			}
		}
	}
	public static void Shuffle<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				list.Swap(i, rnd.Next(i, count));
			}
		}
	}
	public static void ShuffleOne<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			list.Swap(0, rnd.Next(0, list.Count));
		}
	}
	public static void ShuffleOne<T>(this List<T> list, int nItem)
	{
		if (list.Count >= 2 && list.Count >= nItem + 1)
		{
			list.Swap(nItem, rnd.Next(nItem, list.Count));
		}
	}
	public static void ShuffleLast<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			list.Swap(list.Count - 1, rnd.Next(0, list.Count));
		}
	}
	public static T Pop<T>(this List<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}
	public static T Shift<T>(this List<T> list)
	{
		T result = list[0];
		list.RemoveAt(0);
		return result;
	}
	public static T First<T>(this List<T> list)
	{
		return list[0];
	}
	public static T Last<T>(this List<T> list)
	{
		return list[list.Count - 1];
	}

	public static void ShuffleRandomOne<T>(this List<T> list)
	{
		if (list.Count >= 2)
		{
			int num = Randy.randomInt(0, list.Count - 1);
			list.Swap(num, rnd.Next(num, list.Count));
		}
	}
	public static void Swap<T>(this List<T> list, int i, int j)
	{
		(list[i], list[j]) = (list[j], list[i]);
	}
	public static T GetRandom<T>(this List<T> list)
	{
		return list[rnd.Next(0, list.Count)];
	}
	public static void RemoveAtSwapBack<T>(this List<T> list, T pObject)
	{
		int num = list.IndexOf(pObject);
		if (num != -1)
		{
			int index = list.Count - 1;
			list[num] = list[index];
			list[index] = pObject;
			list.RemoveAt(index);
		}
	}
	public static bool Any<T>(this List<T> list)
	{
		if (list == null)
		{
			return false;
		}
		return list.Count > 0;
	}

	public static string ToLineString<T>(this List<T> pList, string pSeparator = ",")
	{
		if (pList == null)
		{
			return string.Empty;
		}
		return string.Join(pSeparator, pList);
	}

	public static void PrintToConsole<T>(this List<T> pList)
	{
		if (pList != null)
		{
			Debug.Log(pList.ToLineString());
		}
	}
	public static void AddTimes<T>(this List<T> pList, int pAmount, T pObject)
	{
		for (int i = 0; i < pAmount; i++)
		{
			pList.Add(pObject);
		}
	}
	public static T LoopNext<T>(this List<T> pList, T pObject)
	{
		int num = pList.IndexOf(pObject);
		if (num == -1)
		{
			return pObject;
		}
		num++;
		if (num >= pList.Count)
		{
			num = 0;
		}
		return pList[num];
	}
    #endregion
    public static Component GetComponent(this GameObject obj, Type type, int index)
    {
        var arr = obj.GetComponents(type.C());
        if(!arr.IsValid()) return null;
        return (Component)arr[index].Cast(type);
    }
    public static T AddComponent<T>(this GameObject gameObject) where T : WrappedBehaviour
    {
        Il2CPPBehaviour behaviour = gameObject.AddComponent<Il2CPPBehaviour>();
        return behaviour.CreateWrapper<T>();
    }
    public static IEnumerable<Transform> GetChildren(this Transform transform)
    {
        for (int i = 0; i < transform.GetChildCount(); i++)
        {
            yield return transform.GetChild(i);
        }
    }

    public static void add(this AssetManager _, BaseMonoLibrary lib, string name)
    {
	    BaseMonoLibrary.add(lib);
    }
    public static void addListener(this NameInput input, Action<string> action)
    {
	    input.addListener(Converter.C<UnityAction<string>>(action));
    }
    public static IPromise Then(this RSG.Promise promise, Func<IPromise> cause)
    {
	    return promise.Then(Converter.C<Il2CppSystem.Func<IPromise>>(cause));
    }
    public static IPromise Then(this RSG.Promise promise, Action cause)
    {
	    return promise.Then(Converter.C<Il2CppSystem.Action>(cause));
    }
    public static IPromise Catch(this IPromise promise, Action<Exception> cause)
    {
	    return promise.Catch(Converter.C<Il2CppSystem.Action<Il2CppSystem.Exception>>((Il2CppSystem.Exception ex) => cause(ex.C())));
    }
    public static IPromise Then(this IPromise promise,Action func, Action<Exception> fail)
    {
	    return promise.Then(Converter.C<Il2CppSystem.Action>(func), Converter.C<Il2CppSystem.Action<Il2CppSystem.Exception>>((Il2CppSystem.Exception exc) => fail(exc.C())));
    }
    public static IPromise Then(this Promise promise,Action func, Action<Exception> fail)
    {
	    return promise.Then(Converter.C<Il2CppSystem.Action>(func), Converter.C<Il2CppSystem.Action<Il2CppSystem.Exception>>((Il2CppSystem.Exception exc) => fail(exc.C())));
    }
    public static void Reject(this RSG.Promise promise, Exception cause)
    {
	    promise.Reject(cause.C());
    }
    public static T GetWrappedComponent<T>(this GameObject obj)
    {
        return (T) WrapperHelper.GetWrappedComponent(obj, typeof(T));
    }
    public static T[] Remove<T>(this T[] arr, T toremove)
    {
        List<T> list = new List<T>(arr);
        list.Remove(toremove);
        return list.ToArray();
    }
    public static void AddListener(this UnityEvent action, Action func){
        action.AddListener(func);
    }
    public static void AddListener<T>(this UnityEvent<T> action, Action<T> func){
        action.AddListener(func);
    }
    public static bool CanAssignTo(this Type derived, Type baseType)
    {
        while (derived != null)
        {
            if (derived == baseType)
                return true;
            
            if (derived.IsGenericType &&
                baseType.IsGenericType &&
                derived.GetGenericTypeDefinition() == baseType.GetGenericTypeDefinition())
                return true;

            derived = derived.BaseType;
        }
        return false;
    }
    public static string FileName(this Assembly assembly)
    {
        return Path.GetFileName(assembly.Location);
    }
    public static string GetInfo(this Type info)
    {
        if (info == null)
        {
            return "Null Type";
        }
        string msg = "";
        if (info.IsGenericType)
        {
            msg = "with generic arguments ";
            foreach (var type in info.GetGenericArguments())
            {
                msg += type.GetInfo() + ", ";
            }
        }
        return $"Type {info.FullName} from {info.Assembly.FileName()} {msg}";
    }
    public static string GetInfo(this CodeInstruction info)
    {
        if (info == null)
        {
            return "Null Instruction";
        }
        string msg = "";
        if (info.operand is MemberInfo member)
        {
            msg = "of ";
            msg += member.GetInfo();
        }
        return $"{info} {msg}";
    }
    public static string GetInfo(this ParameterInfo info)
    {
        if (info == null)
        {
            return "Null Parameter";
        }
        return $"parameter {info.Name} of {info.ParameterType.GetInfo()}";
    }
    public static string GetInfo(this MemberInfo info)
    {
        if (info == null)
        {
            return "Null member";
        }
        return $"member {info.Name} of {info.DeclaringType.GetInfo()}";
    }
    public static string GetInfo(this MethodBase info)
    {
        if(info == null)
        {
            return "Null Method";
        }
        string msg = "";
        foreach (var param in info.GetParameters())
        {
            msg += param.GetInfo();
        }
        return $"method {info.Name} of {info.DeclaringType.GetInfo()} with params {msg}";
    }
    public static WrappedBehaviour AddComponent(this GameObject gameObject, Type type)
    {
        Il2CPPBehaviour behaviour = gameObject.AddComponent<Il2CPPBehaviour>();
        return behaviour.CreateWrapperIfNull(type);
    }
}