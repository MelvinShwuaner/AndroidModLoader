using System.Diagnostics;
using System.Reflection;
using HarmonyLib;
using NeoModLoader.constants;
namespace NeoModLoader.services;
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DoNotDebug : Attribute{}
public static class DebugService
{
    static DebugService()
    {
        Logger = new Debugger<Action<MethodBase, object[]>>(AccessTools.Method(typeof(Hooks), nameof(Hooks.loghook)));
        ExceptionHandler = new Debugger<Action<MethodBase, Exception>>(null, null, AccessTools.Method(typeof(Hooks), nameof(Hooks.finalizer)));
        Profiler = new Debugger<Action<MethodBase, long>>(AccessTools.Method(typeof(Hooks), nameof(Hooks.prefix)), AccessTools.Method(typeof(Hooks), nameof(Hooks.postfix)));
    }
    class Hooks
    {
        public static void loghook(MethodBase __originalMethod, object[] __args)
        {
           Logger.Handler(__originalMethod, __args);
        }
        public static void prefix(out long __state)
        {
            __state = Stopwatch.GetTimestamp();
        }
        public static void postfix(
            MethodBase __originalMethod,
            long __state)
        {
            Profiler.Handler(__originalMethod, Stopwatch.GetTimestamp() - __state);
        }
        public static void finalizer(Exception __exception, MethodBase __originalMethod)
        {
            if (__exception is not null)
                ExceptionHandler.Handler(__originalMethod, __exception);
        }
    }
    public static bool IsDebuggable(MethodBase method)
    {
        if (method.IsAbstract || method.ContainsGenericParameters || method.IsSpecialName || method.Name.Contains("<"))
        {
            return false;
        }
        return true;
    }
    public class Debugger<T> where T : Delegate
    {
        public void AddHandler(T handler)
        {
            Handler = (T)Delegate.Combine(Handler, handler);
        }
        public T Handler { get; private set; }
        public Debugger(MethodInfo Prefix = null, MethodInfo Postfix = null, MethodInfo Finalizer = null)
        {
            if (Prefix is not null)
            {
                this.Prefix = new HarmonyMethod(Prefix);
            }
            if (Postfix is not null)
            {
                this.Postfix = new HarmonyMethod(Postfix);
            }
            if (Finalizer is not null)
            {
                this.Finalizer = new HarmonyMethod(Finalizer);
            }
        }
        HarmonyMethod Prefix;
        HarmonyMethod Postfix;
        HarmonyMethod Finalizer;
        public void Attach(Assembly assembly, Func<MethodBase, bool> predicate = null)
        {
            foreach (var Type in assembly.GetTypes())
            {
                Attach(Type, predicate);
            }
        }
        public void Attach(Type Type, Func<MethodBase, bool> predicate = null)
        {
            predicate ??= DefaultPredicate;
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
    static readonly Harmony Patcher = new (Others.harmony_id);
    public static readonly Debugger<Action<MethodBase, object[]>> Logger;
    public static readonly Debugger<Action<MethodBase, Exception>> ExceptionHandler;
    public static readonly Debugger<Action<MethodBase, long>> Profiler;
    public static readonly Func<MethodBase, bool> DefaultPredicate = method => !method.IsDefined(typeof(DoNotDebug), true) && !method.DeclaringType!.IsDefined(typeof(DoNotDebug), true);
}