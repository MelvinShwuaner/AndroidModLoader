namespace NeoModLoader.AndroidCompatibilityModule;
using Il2CppSystem.Collections;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System = Il2CppSystem;
using Il2CppInterop.Runtime;
using UnityEngine;

/// <summary>
/// collection of tools to allow mods to work on il2cpp and mono on the same code
/// </summary>
public static class Converter
{
    public static D C<D>(Delegate func) where D : System.Delegate
    {
        return DelegateSupport.ConvertDelegate<D>(func);
    }
    public static System.ValueTuple<X, Y, Z> C<X, Y, Z>(this ValueTuple<X, Y, Z> tuple)
    {
        return new System.ValueTuple<X, Y, Z>(tuple.Item1, tuple.Item2, tuple.Item3);
    }
    public static IEnumerator C(this global::System.Collections.IEnumerator enumerator)
    {
        return new IL2CPPEnumerator(enumerator).Cast<IEnumerator>();
    }
    public static System.ValueTuple<X, Y> C<X, Y>(this ValueTuple<X, Y> tuple)
    {
        return new System.ValueTuple<X, Y>(tuple.Item1, tuple.Item2);
    }
    public static System.Type C (this Type type)
    {
        return Il2CppType.From(type);
    }
    public static Type C(this System.Type type)
    {
        var name = type.AssemblyQualifiedName;
        return Type.GetType(name) ?? Type.GetType("Il2Cpp"+name);
    }
    public static MonoException C(this Exception e)
    {
        return new MonoException(e);
    }
    public static Il2CppException C(this System.Exception e)
    {
        return new Il2CppException(e.Pointer);
    }
    public static System.Collections.Generic.HashSet<E> C<E>(this HashSet<E> set)
    {
        System.Collections.Generic.HashSet<E> hash = new();
        foreach (var VARIABLE in set)
        {
            hash.Add(VARIABLE);
        }
        return hash;
    }
    public static HashSet<E> C<E>(this System.Collections.Generic.HashSet<E> set)
    {
        HashSet<E> hash = new();
        foreach (var VARIABLE in set)
        {
            hash.Add(VARIABLE);
        }
        return hash;
    }
    public static System.Collections.Generic.List<E> C<E>(this List<E> e)
    {
        System.Collections.Generic.List<E> list = new System.Collections.Generic.List<E>();
        foreach (var item in e)
        {
            list.Add(item);
        }
        return list;
    }
    public static List<E> C<E>(this System.Collections.Generic.List<E> e)
    {
        List<E> list = new List<E>();
        foreach (var item in e)
        {
            list.Add(item);
        }
        return list;
    }
    public static Dictionary<key, value> C<key, value>(this System.Collections.Generic.Dictionary<key, value> e)
    {
        Dictionary<key, value> dictionary = new Dictionary<key, value>();
        foreach (var item in e)
        {
            dictionary.Add(item.Key, item.Value);
        }
        return dictionary;
    }
    public static System.Collections.Generic.Dictionary<key, value> C<key, value>(this Dictionary<key, value> e)
    {
        System.Collections.Generic.Dictionary<key, value> dictionary = new System.Collections.Generic.Dictionary<key, value>();
        foreach (var item in e)
        {
            dictionary.Add(item.Key, item.Value);
        }
        return dictionary;
    }
}