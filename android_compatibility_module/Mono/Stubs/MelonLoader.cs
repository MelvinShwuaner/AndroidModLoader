using System.Reflection;

namespace MelonLoader;

[AttributeUsage(AttributeTargets.Class)]
public class RegisterTypeInIl2Cpp : Attribute
{
    public static void RegisterAssembly(Assembly assembly)
    {
        
    }
}