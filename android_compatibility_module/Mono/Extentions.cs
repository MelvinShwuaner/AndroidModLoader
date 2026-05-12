using System.Collections;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public static class Extentions
{
    public static T GetWrappedComponent<T>(this GameObject obj)
    {
        return obj.GetComponent<T>();
    }
    public static HashSet<A> Get<A, B>(this SimSystemManager<A, B> manager) where A : BaseSimObject, ILoadable<B>, new() where B : BaseObjectData, new()
    {
        return manager._container._hashSet;
    }
    public static HashSet<A> Get<A, B>(this MetaSystemManager<A, B> manager) where A : MetaObject<B>, new() where B : MetaObjectData, new()
    {
        return manager._hashset;
    }
    public static void AddListener(this UnityEvent ev, Action action)
    {
        ev.AddListener(() => action());
    }
    public static void AddListener<T>(this UnityEvent<T> ev, Action<T> action)
    {
        ev.AddListener((T t) => action(t));
    }
    public static void setToggleAction(this GodPower button, PowerToggleAction action)
    {
        button.toggle_action = action;
    }
    public static GUIStyle Clone(this GUIStyle obj)
    {
        return obj;
    }
    public static List<Transform> GetChildren(this Transform transform)
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform tr in transform)
        {
            list.Add(tr);
        }

        return list;
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
        return $"parameter {info.Name} of {info.ParameterType.GetInfo()}";
    }
    public static string GetInfo(this MemberInfo info)
    {
        return $"member {info.Name} of {info.DeclaringType.GetInfo()}";
    }
    public static string GetInfo(this MethodBase info)
    {
        string msg = "";
        foreach (var param in info.GetParameters())
        {
            msg += param.GetInfo();
        }
        return $"member {info.Name} of {info.DeclaringType.GetInfo()} with params {msg}";
    }
}