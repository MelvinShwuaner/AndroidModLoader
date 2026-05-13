using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
namespace NeoModLoader.AndroidCompatibilityModule;
/// <summary>
/// common functions for IL2CPP-Mono Specific functions
/// </summary>
public static class IL2CPPHelper
{
    public static GameObject CreateGameObject(string name, params Type[] types)
    {
        Il2CppSystem.Type[] Types = new Il2CppSystem.Type[types.Length];
        List<Type> WrappedTypes = new List<Type>();
        for(int i = 0; i< types.Length; i++)
        {
            if(typeof(WrappedBehaviour).IsAssignableFrom(types[i]))
            {
                WrappedTypes.Add(types[i]);
                Types[i] = Il2CppType.Of<Il2CPPBehaviour>();
            }
            else
            {
                Types[i] = types[i].C();
            }
        }
        GameObject obj = new GameObject(name, Types);
        if (WrappedTypes.Count <= 0) return obj;
        var behs = obj.GetComponents<Il2CPPBehaviour>();
        for (int i = 0; i < WrappedTypes.Count; i++)
        {
            behs[i].CreateWrapperIfNull(WrappedTypes[i]);
        }
        return obj;
    }

    public static TextAsset CreateTextAsset(string content, string name)
    {
        TextAsset textAsset = new TextAsset(TextAsset.CreateOptions.CreateNativeObject, content);
        textAsset.name = name;
        return textAsset;
    }
    public static D C<D>(Delegate func) where D : Il2CppSystem.Delegate
    {
        return DelegateSupport.ConvertDelegate<D>(func);
    }
    public static Il2CppReferenceArray<A> A<A>(params A[] arr) where A : Il2CppObjectBase
    {
        return arr;
    }
    public static Il2CppStringArray A(params string[] arr)
    {
        return arr;
    }
    public static Sys.Collections.Generic.List<T> L<T>(params T[] arr)
    {
        Sys.Collections.Generic.List<T> list = new();
        foreach (var t in arr)
        {
            list.Add(t);
        }

        return list;
    }
    public static Il2CppObjectBase Cast(this Il2CppObjectBase obj, Type type)
    {
        var method = typeof(Il2CppObjectBase)
            .GetMethod("Cast")!
            .MakeGenericMethod(type);
        return (Il2CppObjectBase)method.Invoke(obj, null);
    }
    public static Sys.Collections.Generic.HashSet<T> H<T>(params T[] arr)
    {
        Sys.Collections.Generic.HashSet<T> list = new();
        foreach (var t in arr)
        {
            list.Add(t);
        }

        return list;
    }
}