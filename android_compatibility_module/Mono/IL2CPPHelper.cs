using UnityEngine;

namespace NeoModLoader.AndroidCompatibilityModule;

public static class IL2CPPHelper
{
    public static GameObject CreateGameObject(string name, params Type[] types)
    {
        return new GameObject(name, types);
    }
    public static TextAsset CreateTextAsset(string content, string name)
    {
        TextAsset textAsset = new TextAsset(content);
        textAsset.name = name;
        return textAsset;
    }
    public static A[] A<A>(params A[] a)
    {
        return a;
    }
    public static A? N<A>(this A a) where A : struct
    {
        return a;
    }
    public static System.Collections.Generic.List<T> L<T>(params T[] arr)
    {
        System.Collections.Generic.List<T> list = new();
        foreach (var t in arr)
        {
            list.Add(t);
        }

        return list;
    }
    public static System.Collections.Generic.HashSet<T> H<T>(params T[] arr)
    {
        System.Collections.Generic.HashSet<T> list = new();
        foreach (var t in arr)
        {
            list.Add(t);
        }

        return list;
    }
    public static T Cast<T>(this object obj)
    {
        return (T)obj;
    }
}