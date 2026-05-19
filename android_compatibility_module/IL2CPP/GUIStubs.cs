using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using NeoModLoader.constants;
using NeoModLoader.services;
using UnityEngine;
using Object = UnityEngine.Object;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace NeoModLoader.AndroidCompatibilityModule;

internal static class GUIStubs //fixes GUI stub methods because il2cppinterops bitch ass couldn't do it itself
{
    private static Harmony harmony;

    static void Transpile(MethodBase method, Delegate @delegate)
    {
        harmony.Patch(method, null, null, new HarmonyMethod(@delegate));
    }
    public static void Init()
    {
        harmony = new Harmony(Others.harmony_id);
        Transpile(AccessTools.Method(typeof(GUILayout), nameof(GUILayout.Space)), SpaceFix);
        Transpile(AccessTools.Method(typeof(GUI), nameof(GUI.DrawTexture), [typeof(Rect), typeof(Texture), typeof(ScaleMode), typeof(bool), typeof(float), typeof(Color), typeof(Color), typeof(Color), typeof(Color), typeof(Vector4), typeof(Vector4), typeof(bool)]), DrawTextureFix);
        Transpile(AccessTools.Method(typeof(GUILayout), nameof(GUILayout.FlexibleSpace)), FlexibleSpaceFix); 
        harmony.PatchAll(typeof(Patches));
    }
    static void Space(float pixels)
    {
        GUIUtility.CheckOnGUI();
        if (GUILayoutUtility.current.topLevel.isVertical)
            GUILayoutUtility.GetRect(0.0f, pixels, spaceStyle, GUILayout.Height(pixels));
        else
            GUILayoutUtility.GetRect(pixels, 0.0f, spaceStyle, GUILayout.Width(pixels));
        if (Event.current.type != EventType.Layout)
            return;
        GUILayoutUtility.current.topLevel.entries[GUILayoutUtility.current.topLevel.entries.Count - 1].consideredForMargin = false;
    }
    static void DrawTexture(
        Rect position,
        Texture image,
        ScaleMode scaleMode,
        bool alphaBlend,
        float imageAspect,
        Color leftColor,
        Color topColor,
        Color rightColor,
        Color bottomColor,
        Vector4 borderWidths,
        Vector4 borderRadiuses,
        bool drawSmoothCorners)
    {
        GUIUtility.CheckOnGUI();
        if (Event.current.type != EventType.Repaint)
            return;
        if (image == null)
        {
            Debug.LogWarning("null texture passed to GUI.DrawTexture");
        }
        else
        {
            /*if (imageAspect == 0.0)
                imageAspect = image.width / (float) image.height;
            Material material = !(borderWidths != Vector4.zero) ? (!(borderRadiuses != Vector4.zero) ? (alphaBlend ? GUI.blendMaterial : GUI.blitMaterial) : GUI.roundedRectMaterial) : (!(leftColor != topColor) && !(leftColor != rightColor) && !(leftColor != bottomColor) ? GUI.roundedRectMaterial : GUI.roundedRectWithColorPerBorderMaterial);
            Internal_DrawTextureArguments args = new Internal_DrawTextureArguments()
            {
                leftBorder = 0,
                rightBorder = 0,
                topBorder = 0,
                bottomBorder = 0,
                color = leftColor,
                leftBorderColor = leftColor,
                topBorderColor = topColor,
                rightBorderColor = rightColor,
                bottomBorderColor = bottomColor,
                borderWidths = borderWidths,
                cornerRadiuses = borderRadiuses,
                texture = image,
                smoothCorners = drawSmoothCorners,
                mat = material
            };
            var argsScreenRect = args.screenRect;
            var argsSourceRect = args.sourceRect;
            GUI.CalculateScaledTextureRects(position, scaleMode, imageAspect, ref argsScreenRect, ref argsSourceRect);
            args.screenRect = argsScreenRect;
            args.sourceRect = argsSourceRect;
            Graphics.Internal_DrawTexture(ref args); */ //doesnt fucking work
            Graphics.DrawTexture(position, image);
        }
    }
    static IEnumerable<CodeInstruction> SpaceFix(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldarg_0);
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GUIStubs), nameof(Space)));
        yield return new CodeInstruction(OpCodes.Ret);
    }
    static IEnumerable<CodeInstruction> DrawTextureFix(IEnumerable<CodeInstruction> instructions)
    {
        for (int i = 0; i < 12; i++) //kill me
        {
            yield return new CodeInstruction(OpCodes.Ldarg, i);
        }
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GUIStubs), nameof(DrawTexture)));
        yield return new CodeInstruction(OpCodes.Ret);
    }
    static GUIStyle spaceStyle
    {
        get
        {
            field ??= new GUIStyle();
            field.stretchWidth = false;
            return field;
        }
    }
    public static void FlexibleSpace()
    {
        GUIUtility.CheckOnGUI();
        GUILayoutUtility.GetRect(0.0f, 0.0f, spaceStyle, new GUILayoutOption((!GUILayoutUtility.current.topLevel.isVertical ? GUILayout.ExpandWidth(true) : GUILayout.ExpandHeight(true)).type, 10000));
        if (Event.current.type != EventType.Layout)
            return;
        GUILayoutUtility.current.topLevel.entries[GUILayoutUtility.current.topLevel.entries.Count - 1].consideredForMargin = false;
    }
    static IEnumerable<CodeInstruction> FlexibleSpaceFix(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GUIStubs), nameof(FlexibleSpace)));
        yield return new CodeInstruction(OpCodes.Ret);
    }
    public static Rect DoWindow(
        int id,
        Rect screenRect,
        GUI.WindowFunction func,
        GUIContent content,
        GUIStyle style,
        Il2CppReferenceArray<GUILayoutOption> options)
    {
        GUIUtility.CheckOnGUI();
        layoutedWindow = new LayoutedWindow(func, screenRect, content, options, style);
        return GUI.Window(id, screenRect, IL2CPPHelper.C<GUI.WindowFunction>(TempDoWindow), content, style);
    }
    private static LayoutedWindow layoutedWindow;
    static void TempDoWindow(int id)
    {
        layoutedWindow.DoWindow(id);
    }
    private sealed class LayoutedWindow
    {
        private readonly GUI.WindowFunction m_Func;
        private readonly Rect m_ScreenRect;
        private readonly GUILayoutOption[] m_Options;
        private readonly GUIStyle m_Style;

        internal LayoutedWindow(
            GUI.WindowFunction f,
            Rect screenRect,
            GUIContent content,
            GUILayoutOption[] options,
            GUIStyle style)
        {
            this.m_Func = f;
            this.m_ScreenRect = screenRect;
            this.m_Options = options;
            this.m_Style = style;
        }

        public void DoWindow(int windowID)
        {
            GUILayoutGroup topLevel = GUILayoutUtility.current.topLevel;
            if (Event.current.type == EventType.Layout)
            {
                topLevel.resetCoords = true;
                topLevel.rect = this.m_ScreenRect;
                if (this.m_Options != null)
                    topLevel.ApplyOptions(this.m_Options);
                topLevel.isWindow = true;
                topLevel.windowID = windowID;
                topLevel.style = this.m_Style;
            }
            else
                topLevel.ResetCursor();
            this.m_Func.Invoke(windowID);
        }
    }

    static class Patches//cuz the original functions dont fucking work for the most BS reason
    {
        [HarmonyPatch(typeof(TextEditor), nameof(TextEditor.SaveBackup))]
        [HarmonyPrefix]
        public static bool SaveBackup(TextEditor __instance)
        {
            return false;
        }
        [HarmonyPatch(typeof(TextEditor), nameof(TextEditor.DetectFocusChange))]
        [HarmonyPrefix]
        public static bool DetectFocusChange(TextEditor __instance)
        {
            return false;
        }
        [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.MinWidth))]
        [HarmonyPrefix]
        public static bool MinWidth(float minWidth, ref GUILayoutOption __result) 
        {
            __result = new GUILayoutOption(GUILayoutOption.Type.minWidth, minWidth);
            return false;
        }

        [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.MaxWidth))]
        [HarmonyPrefix]
        public static bool MaxWidth(float maxWidth, ref GUILayoutOption __result) 
        {
            __result = new GUILayoutOption(GUILayoutOption.Type.maxWidth, maxWidth);
            return false;
        }

        [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.MinHeight))]
        [HarmonyPrefix]
        public static bool MinHeight(float minHeight, ref GUILayoutOption __result) 
        {
            __result = new GUILayoutOption(GUILayoutOption.Type.minHeight, minHeight);
            return false;
        }

        [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.MaxHeight))]
        [HarmonyPrefix]
        public static bool MaxHeight(float maxHeight, ref GUILayoutOption __result) 
        {
            __result = new GUILayoutOption(GUILayoutOption.Type.maxHeight, maxHeight);
            return false;
        }

        [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.ExpandWidth))]
        [HarmonyPrefix]
        public static bool ExpandWidth(bool expand, ref GUILayoutOption __result) 
        {
            __result = new GUILayoutOption(GUILayoutOption.Type.stretchWidth, expand ? 1 : 0);
            return false;
        }

        [HarmonyPatch(typeof(GUILayout), nameof(GUILayout.ExpandHeight))]
        [HarmonyPrefix]
        public static bool ExpandHeight(bool expand, ref GUILayoutOption __result) 
        {
            __result = new GUILayoutOption(GUILayoutOption.Type.stretchHeight, expand ? 1 : 0);
            return false;
        }
    }
}