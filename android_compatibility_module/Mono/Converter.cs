using System.Collections;
using UnityEngine;

namespace NeoModLoader.AndroidCompatibilityModule;

public static class Converter
{
    public static A C<A>(this A a)
    {
        return a;
    }
     public static D C<D>(Delegate func) where D : System.Delegate
     {
         return (D)func;
     }
}