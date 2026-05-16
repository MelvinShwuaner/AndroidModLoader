using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
namespace NeoModLoader.AndroidCompatibilityModule;

public class IL2CPPBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public B SetWrappedBehaviour<B>(B Behaviour) where B : WrappedBehaviour
    {
        throw new PlatformNotSupportedException();
    }
    [HideFromIl2Cpp]
    public WrappedBehaviour CreateWrapperIfNull(Type WrappedType)
    {
        throw new PlatformNotSupportedException();
    }
    [HideFromIl2Cpp]
    public W CreateWrapper<W>() where W : WrappedBehaviour
    {
        throw new PlatformNotSupportedException();
    }
}