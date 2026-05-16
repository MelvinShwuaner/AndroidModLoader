using System.Reflection;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using MelonLoader;
using NeoModLoader.utils.Collections;
using UnityEngine;

namespace NeoModLoader.AndroidCompatibilityModule;
[RegisterTypeInIl2Cpp]
public sealed class Il2CPPBehaviour : MonoBehaviour
{
    public Il2CPPBehaviour(IntPtr ptr) : base(ptr)
    {
    }

    public Il2CPPBehaviour() : base(ClassInjector.DerivedConstructorPointer<Il2CPPBehaviour>())
    {
        ClassInjector.DerivedConstructorBody(this);
    }

    public void OnEnable()
    {
        onenable?.Invoke(WrappedBehaviour);
    }

    public void Start()
    {
       start?.Invoke(WrappedBehaviour);
    }

    public void OnDisable()
    {
        ondisable?.Invoke(WrappedBehaviour);
    }

    private bool canawake;
    public void Awake()
    {
        if (!canawake) return;
        awake?.Invoke(WrappedBehaviour);
        canawake = false;
    }
    public void OnDestroy()
    {
        ondestroy?.Invoke(WrappedBehaviour);
    }

    public void Update()
    {
        WrappedBehaviour.HandleInvokations(Time.deltaTime);
        update?.Invoke(WrappedBehaviour);
    }

    public void LateUpdate()
    {
        lateupdate?.Invoke(WrappedBehaviour);
    }

    public void OnGUI()
    {
        ongui?.Invoke(WrappedBehaviour);
    }
    [HideFromIl2Cpp]
    WrappedAction GetWrappedMethod(string Method)
    {
        return WrappedMethodCollection.Get(WrappedType)[Method];
    }
    [HideFromIl2Cpp]
    public B SetWrappedBehaviour<B>(B Behaviour) where B : WrappedBehaviour
    {
        WrappedBehaviour = Behaviour;
        WrappedType = Behaviour.GetType();
        Behaviour.Wrapper = this;
        update = GetWrappedMethod("Update");
        start = GetWrappedMethod("Start");
        awake = GetWrappedMethod("Awake");
        ongui = GetWrappedMethod("OnGUI");
        onenable = GetWrappedMethod("OnEnable");
        ondisable = GetWrappedMethod("OnDisable");
        lateupdate = GetWrappedMethod("LateUpdate");
        ondestroy = GetWrappedMethod("OnDestroy");
        canawake = true;
        if (gameObject.activeInHierarchy)
        {
            Awake();
        }
        return Behaviour;
    }
    [HideFromIl2Cpp]
    public WrappedBehaviour CreateWrapperIfNull(Type WrappedType)
    {
        return WrappedBehaviour ?? SetWrappedBehaviour((WrappedBehaviour)Activator.CreateInstance(WrappedType));
    }
    [HideFromIl2Cpp]
    public W CreateWrapper<W>() where W : WrappedBehaviour
    {
        return SetWrappedBehaviour(Activator.CreateInstance<W>());
    }
    private WrappedAction update;
    private WrappedAction start;
    private WrappedAction awake;
    private WrappedAction ongui;
    private WrappedAction onenable;
    private WrappedAction ondisable;
    private WrappedAction lateupdate;
    private WrappedAction ondestroy;
    public Type WrappedType { [HideFromIl2Cpp] get; [HideFromIl2Cpp] private set; }
    public WrappedBehaviour WrappedBehaviour { [HideFromIl2Cpp] get; [HideFromIl2Cpp] private set; }
}
public delegate void WrappedAction(WrappedBehaviour instance);