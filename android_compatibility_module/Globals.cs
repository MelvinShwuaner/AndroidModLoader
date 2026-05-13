#if IL2CPP
global using ObjectArray = Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Object>;
global using Sys = Il2CppSystem;
#else
global using ObjectArray = UnityEngine.Object[];
global using Sys = System;
#endif