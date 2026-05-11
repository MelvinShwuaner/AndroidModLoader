using UnityEngine;

namespace NeoModLoader.AndroidCompatibilityModule;

public class SmoothLoaderHelper
{
    #if IL2CPP
    public static void add(
        Action pAction,
        string pId,
        bool pSkipFrame = false,
        float pNewWaitTimerValue = 0.001f,
        bool pToEnd = false)
    {
        SmoothLoader.add(pAction, pId, pSkipFrame, pNewWaitTimerValue, pToEnd);
    }
    #endif
    public static void add(
        MapLoaderAction pAction,
        string pId,
        bool pSkipFrame = false,
        float pNewWaitTimerValue = 0.001f,
        bool pToEnd = false)
    {
        SmoothLoader.add(pAction, pId, pSkipFrame, pNewWaitTimerValue, pToEnd);
    }
}
public static class GUIHelper
{
    public static class Layout{
#if IL2CPP
        public static Rect Window(int id, Rect clientRect, Action<int>  func, string text)
        {
            return GUILayout.Window(id, clientRect, func, text);
        }
        public static Rect Window(int id, Rect clientRect, Action<int>  func, GUIContent text)
        {
            return GUILayout.Window(id, clientRect, func, text);
        }
#endif
        public static Rect Window(int id, Rect clientRect, GUI.WindowFunction  func, string text)
        {
            return GUILayout.Window(id, clientRect, func, text);
        }
        public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, GUIContent text)
        {
            return GUILayout.Window(id, clientRect, func, text);
        }
    }
    #if IL2CPP
    public static Rect Window(int id, Rect clientRect, Action<int>  func, string text)
    {
        return GUI.Window(id, clientRect, func, text);
    }
    public static Rect Window(int id, Rect clientRect, Action<int>  func, GUIContent text)
    {
        return GUI.Window(id, clientRect, func, text);
    }
    #endif
    public static Rect Window(int id, Rect clientRect, GUI.WindowFunction  func, string text)
    {
        return GUI.Window(id, clientRect, func, text);
    }
    public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, GUIContent text)
    {
        return GUI.Window(id, clientRect, func, text);
    }
}

public static class ActionHelper
{
    public static readonly WorldAction Default = Converter.C<WorldAction>((BaseSimObject _, WorldTile _) => true);
}