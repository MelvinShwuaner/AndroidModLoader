using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using NeoModLoader.constants;

namespace NeoModLoader.services;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DebugAttribute : Attribute{}
public static class DebugService
{
    static bool IsDebuggable(MethodBase method)
    {
        if (method.IsAbstract || method.ContainsGenericParameters || method.IsSpecialName || method.Name.Contains("<"))
        {
            return false;
        }
        return true;
    }
    public abstract class Debugger
    {
        protected HarmonyMethod Prefix;
        protected HarmonyMethod Postfix;
        protected HarmonyMethod Finalizer;
        public void Attach(Assembly assembly, Func<MethodBase, bool> predicate = null)
        {
            foreach (var Type in assembly.GetTypes())
            {
                Attach(Type, predicate);
            }
        }
        public void Attach(Type Type, Func<MethodBase, bool> predicate = null)
        {
            predicate ??= Default;
            foreach (var method in Type.GetMethods(
                         BindingFlags.Public |
                         BindingFlags.NonPublic |
                         BindingFlags.Instance |
                         BindingFlags.Static |
                         BindingFlags.DeclaredOnly).Where(m => IsDebuggable(m) && predicate(m)))
            {
                Attach(method);
            }
            foreach (var method in Type.GetConstructors(
                         BindingFlags.Public |
                         BindingFlags.NonPublic |
                         BindingFlags.Instance |
                         BindingFlags.Static |
                         BindingFlags.DeclaredOnly).Where(m => IsDebuggable(m) && predicate(m)))
            {
                Attach(method);
            }
        }
        public void Attach(MethodBase method)
        {
            try
            {
                Patcher.Patch(method, Prefix, Postfix, null, Finalizer, null);
            }
            catch (Exception e)
            {
                LogService.LogError($"Failed to attach debugger to {method.FullDescription()} due to {e}");
            }
        }
    }
    public class LogDebugger : Debugger
    {
        static void prefix(MethodBase __originalMethod)
        {
            LogService.Log(__originalMethod.ToString());
        }
        public LogDebugger()
        {
            Prefix = new HarmonyMethod(AccessTools.Method(typeof(LogDebugger), nameof(prefix)));
        }
    }
    public class ProfilerDebugger : Debugger
    {
        static Stopwatch stopwatch = new();
        public static void prefix(out long __state)
        {
            __state = Stopwatch.GetTimestamp();
        }
        public static void postfix(
            MethodBase __originalMethod,
            long __state)
        {
            LogService.Log(
                $"{__originalMethod.Name} took {Stopwatch.GetTimestamp() - __state}");
        }
        public ProfilerDebugger()
        {
            Prefix = new HarmonyMethod(AccessTools.Method(typeof(ProfilerDebugger), nameof(prefix)));
            Postfix = new HarmonyMethod(AccessTools.Method(typeof(ProfilerDebugger), nameof(postfix)));
        }
    }
    public class ExceptionDebugger : Debugger
    {
        static void finalizer(Exception __exception, MethodBase __original)
        {
            if (__exception is not null)
               handler?.Invoke(__exception, __original);
        }
        public ExceptionDebugger()
        {
            Finalizer = new HarmonyMethod(AccessTools.Method(typeof(ExceptionDebugger), nameof(finalizer)));
        }
        public static void AddHandler(ExceptionHandler Handler)
        {
            handler += Handler;
        }
        public delegate void ExceptionHandler(Exception Exception, MethodBase Method);
        public static event ExceptionHandler handler;
    }
    static readonly Harmony Patcher = new Harmony(Others.harmony_id);
    public static readonly LogDebugger Logger = new();
    public static readonly ProfilerDebugger Profiler = new();
    public static readonly ExceptionDebugger ExceptionHandler = new();
    static readonly Func<MethodBase, bool> Default = _ => true;
    static readonly Func<MethodBase, bool> Attribute = method => method.IsDefined(typeof(DebugAttribute), true) || method.DeclaringType?.IsDefined(typeof(DebugAttribute), true) == true;
}