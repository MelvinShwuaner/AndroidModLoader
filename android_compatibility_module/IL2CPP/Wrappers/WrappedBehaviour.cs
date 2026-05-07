using System.Collections;
using Il2CppInterop.Runtime;
using NeoModLoader.services;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NeoModLoader.AndroidCompatibilityModule;

public class WrappedBehaviour
{
    [JsonIgnore]
    public Transform transform => Wrapper.transform;
    [JsonIgnore]
    public GameObject gameObject => Wrapper.gameObject;
    [JsonIgnore]
    public string name
    {
        get => Wrapper.name;
        set => Wrapper.name = value;
    }

    public static void DontDestroyOnLoad(GameObject gameObject)
    {
        Il2CPPBehaviour.DontDestroyOnLoad(gameObject);
    }

    public static T FindObjectOfType<T>(bool includeInactive = false) where T : Object
    {
        var obj = Object.FindObjectOfType(Il2CppType.Of<T>(), includeInactive);
        if (obj == null)
            return null;
        return obj.Cast<T>();
    }
    public static T FindObjectOfType<T>(bool includeInactive = false, bool stub = true) where T : WrappedBehaviour
    {
        Il2CPPBehaviour[] il2cpp = FindObjectsOfType<Il2CPPBehaviour>(includeInactive);
        Type type = typeof(T);
        foreach (var beh in il2cpp)
        {
            if (beh.WrappedType.IsAssignableTo(type))
            {
                return (T)beh.WrappedBehaviour;
            }
        }
        return null;
    }
    public static T[] FindObjectsOfType<T>(bool includeInactive = false, bool stub = true) where T : WrappedBehaviour
    {
        List<T> list = new List<T>();
        Il2CPPBehaviour[] il2cpp = FindObjectsOfType<Il2CPPBehaviour>(includeInactive);
        Type type = typeof(T);
        foreach (var beh in il2cpp)
        {
            if (beh.WrappedType.IsAssignableTo(type))
            {
                list.Add((T)beh.WrappedBehaviour);
            }
        }

        return list.ToArray();
    }
    public static T[] FindObjectsOfType<T>(bool includeInactive = false) where T : Object
    {
        var arr = Object.FindObjectsOfType(Il2CppType.Of<T>(), includeInactive);
        if (!arr.IsValid())
            return [];
        return arr
            .Select(obj => obj.Cast<T>())
            .ToArray();
    }
    [JsonIgnore]
    public Il2CPPBehaviour Wrapper { get; internal set; }
    public C GetComponent<C>()
    {
        return Wrapper.GetComponent<C>();
    }
    public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays = true, bool stub = true) where T : WrappedBehaviour
    {
        return WrapperHelper.Instantiate(original, parent, worldPositionStays);
    }
    public Coroutine StartCoroutine(IEnumerator enumerator)
    {
        return Wrapper.StartCoroutine(enumerator.ToIL2CPP());
    }
    public void StopCoroutine(Coroutine coroutine)
    {
        Wrapper.StopCoroutine(coroutine);
    }
    public void StopAllCoroutines(){
        Wrapper.StopAllCoroutines();
    }
    public static T Instantiate<T>(T obj, Transform parent = null, bool positionstays = false) where T : Object
    {
        return Object.Instantiate(obj, parent, positionstays);
    }
    public static void Destroy(UnityEngine.Object Object)
    {
        GameObject.Destroy(Object);
    }

    public T AddComponent<T>() where T : Component
    {
        return Wrapper.AddComponent<T>();
    }
    public T AddComponent<T>(bool stub = true) where T : WrappedBehaviour
    {
        return gameObject.AddComponent<T>();
    }
    public static implicit operator Il2CPPBehaviour(WrappedBehaviour beh)
    {
        return beh.Wrapper;
    }
    public static implicit operator WrappedBehaviour(Il2CPPBehaviour beh)
    {
        return beh.WrappedBehaviour;
    }
}