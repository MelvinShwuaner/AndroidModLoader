#if IL2CPP
global using ObjectArray = Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object>;
global using SysType = Il2CppSystem.Type;
#else
global using ObjectArray = UnityEngine.Object[];
global using SysType = System.Type;
#endif