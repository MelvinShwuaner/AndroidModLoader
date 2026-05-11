using System.Reflection;
using HarmonyLib;
using NeoModLoader.constants;

namespace NeoModLoader.services;

public static class DebugService
{
    static void Debugger(MethodBase __originalMethod)
    {
        LogService.LogInfo(__originalMethod.ToString());
    }
    static readonly Harmony Patcher = new Harmony(Others.harmony_id);
    private static readonly HarmonyMethod Hook = new(AccessTools.Method(typeof(DebugService), nameof(Debugger)));
    public static void AttachDebugger(Assembly assembly)
    {
        foreach (var Type in assembly.GetTypes())
        {
            foreach (var method in Type.GetMethods(
                         BindingFlags.Public |
                         BindingFlags.NonPublic |
                         BindingFlags.Instance |
                         BindingFlags.Static |
                         BindingFlags.DeclaredOnly))
            {
                Patcher.Patch(method, Hook);
            }
        }
    }
    public static void RemoveDebugger(Assembly assembly)
    {
        foreach (var Type in assembly.GetTypes())
        {
            foreach (var method in Type.GetMethods(
                         BindingFlags.Public |
                         BindingFlags.NonPublic |
                         BindingFlags.Instance |
                         BindingFlags.Static |
                         BindingFlags.DeclaredOnly))
            {
                Patcher.Unpatch(method, HarmonyPatchType.Prefix);
            }
        }
    }
}