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
        public static Rect Window(int id, Rect clientRect, Action<int>  func, string text, params GUILayoutOption[] options)
        {
            return GUIStubs.DoWindow(id, clientRect, func, GUIContent.Temp(text), GUI.skin.window, options);
        }
        public static Rect Window(int id, Rect clientRect, Action<int>  func, GUIContent text, params GUILayoutOption[] options)
        {
            return GUIStubs.DoWindow(id, clientRect, func, text, GUI.skin.window, options);
        }
#endif
        public static Rect Window(int id, Rect clientRect, GUI.WindowFunction  func, string text,  params GUILayoutOption[] options)
        {
            return GUILayout.Window(id, clientRect, func, text, options);
        }
        public static Rect Window(int id, Rect clientRect, GUI.WindowFunction func, GUIContent text, params GUILayoutOption[] options)
        {
            return GUILayout.Window(id, clientRect, func, text, options);
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
    public static readonly WorldAction Default = IL2CPPHelper.C<WorldAction>((BaseSimObject _, WorldTile _) => true);
}