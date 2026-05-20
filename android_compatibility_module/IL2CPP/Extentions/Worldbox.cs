
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using NeoModLoader.AndroidCompatibilityModule;

public static partial class Extentions
{
    public static void addListener(this NameInput input, Action<string> action)
    {
        input.addListener(action);
    }
    public static void addOpposites<T>(this BaseTrait<T> trait, IEnumerable<string> pListIDS) where T : BaseTrait<T>
    {
        trait.addOpposites(pListIDS.AsList().AsEnumerable<string>());
    }
    public static void add(this AssetManager _, BaseMonoLibrary lib, string name)
    {
        BaseMonoLibrary.add(lib);
    }
    public static void setHoverAction(this TipButton button, Action action)
    {
        button.hoverAction = action;
    }

    public static void setToggleAction(this GodPower button, Action<string> action)
    {
        button.toggle_action = action;
    }
    public static void addGenome(this ActorAsset asset, params ValueTuple<string, float>[] pListGenomePartsIDs)
    {
        return; //what the fuck
        Il2CppReferenceArray<Il2CppSystem.ValueTuple<string, float>> arr = new Il2CppReferenceArray<Il2CppSystem.ValueTuple<string, float>>((long)pListGenomePartsIDs.Length);
        for (var i = 0; i < pListGenomePartsIDs.Length; i++)
        {
            arr[i] = pListGenomePartsIDs[i].C();
        }
        asset.addGenome(arr);
    }
    public static Il2CppSystem.Collections.Generic.HashSet<A> Get<A, B>(this SimSystemManager<A, B> manager) where A : BaseSimObject, new() where B : BaseObjectData, new()
    {
        return manager._container._hashSet;
    }
    public static Il2CppSystem.Collections.Generic.HashSet<A> Get<A, B>(this CoreSystemManager<A, B> manager) where A : CoreSystemObject<B>, new() where B : BaseSystemData, new()
    {
        return manager._hashset;
    }
    public static void doUnits(this WorldTile tile, Action<Actor> action)
    {
        tile.doUnits(action);
    }
}