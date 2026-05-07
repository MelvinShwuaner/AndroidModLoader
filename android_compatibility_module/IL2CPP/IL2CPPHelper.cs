using Il2CppInterop.Runtime;
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
        TextAsset textAsset = new TextAsset(TextAsset.CreateOptions.None, content);
        textAsset.name = name;
        return textAsset;
    }
}