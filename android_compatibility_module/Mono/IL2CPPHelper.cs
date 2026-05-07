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
}