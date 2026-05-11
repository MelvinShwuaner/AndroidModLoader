using RSG;

namespace NeoModLoader.AndroidCompatibilityModule;

public static partial class Extentions
{
    public static IPromise Then(this RSG.Promise promise, Func<IPromise> cause)
    {
        return promise.Then(cause);
    }
    public static IPromise Then(this RSG.Promise promise, Action cause)
    {
        return promise.Then(cause);
    }
    public static IPromise Catch(this IPromise promise, Action<Exception> cause)
    {
        return promise.Catch((Action<Il2CppSystem.Exception>)(ex => cause(ex.C())));
    }
    public static IPromise Then(this IPromise promise,Action func, Action<Exception> fail)
    {
        return promise.Then(func, (Action<Il2CppSystem.Exception>)(exc => fail(exc.C())));
    }
    public static IPromise Then(this Promise promise,Action func, Action<Exception> fail)
    {
        return promise.Then(func, (Action<Il2CppSystem.Exception>)(exc => fail(exc.C())));
    }
    public static void Reject(this RSG.Promise promise, Exception cause)
    {
        promise.Reject(cause.C());
    }
}