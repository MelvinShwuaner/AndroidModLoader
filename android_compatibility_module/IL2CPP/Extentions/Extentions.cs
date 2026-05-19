using System.Collections;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using NeoModLoader.services;
using UnityEngine;
using UnityEngine.Events;
using NeoModLoader.AndroidCompatibilityModule;
public static partial class Extentions
{
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
    public static nint Clone(this GUIStyle orig)
    {
	    GUIStyle style = new GUIStyle(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<GUIStyle>.NativeClassPtr));
	    style.m_Ptr = GUIStyle.Internal_Copy(style, orig);
	    return style.Pointer;
    }
    /// <summary>
    /// casts objects using il2cpp
    /// </summary>
    public static IEnumerable<T> OfType<T>(this IEnumerable<Il2CppObjectBase> list)where T : Il2CppObjectBase
    {
        foreach (Il2CppObjectBase obj in list)
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
	    return type!.IsPrimitive || type == typeof(string);
    }
    //functions like listextention are useless to us now
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
    public static Coroutine StartCoroutine(this MonoBehaviour beh, IEnumerator enumerable)
    {
        return beh.StartCoroutine(enumerable.C());
    }
    public static IEnumerable<Transform> GetChildren(this Transform transform)
    {
        for (int i = 0; i < transform.GetChildCount(); i++)
        {
            yield return transform.GetChild(i);
        }
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
    public static WrappedBehaviour AddComponent(this GameObject gameObject, Type type)
    {
        Il2CPPBehaviour behaviour = gameObject.AddComponent<Il2CPPBehaviour>();
        return behaviour.CreateWrapperIfNull(type);
    }
    //TODO: get rid of this BS
    #region  TranspilerBullShit
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
    #endregion
}