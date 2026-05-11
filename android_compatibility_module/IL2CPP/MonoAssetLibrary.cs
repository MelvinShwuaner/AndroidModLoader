namespace NeoModLoader.AndroidCompatibilityModule;

public abstract class MonoAsset
{
    public string id = "ASSET_ID";
    public virtual void create()
    {
    }
    public void setHash(int pHash) => this._hashcode = pHash;
    public int _hashcode;
    public bool isTemplateAsset() => this.id.StartsWith("$") || this.id.StartsWith("_");
    public void setIndexID(int pValue) => this._index = pValue;
    public int _index;
}
public class MonoAssetLibrary<T> : BaseMonoLibrary where T : MonoAsset
{
    public T t;
    public List<T> list = new List<T>();
    public Dictionary<string, T> dict = new Dictionary<string, T>();
    public virtual T add(T pAsset)
    {
        string id = pAsset.id;
        if (this.dict.ContainsKey(id))
        {
            for (int index = 0; index < this.list.Count; ++index)
            {
                if (!(this.list[index].id != id))
                {
                    this.list.RemoveAt(index);
                    break;
                }
            }
            this.dict.Remove(id);
            BaseAssetLibrary.logAssetError($"<e>AssetLibrary<{typeof (T).Name}></e>: duplicate asset - overwriting...", id);
        }
        this.t = pAsset;
        this.t.create();
        this.t.setHash(BaseAssetLibrary._latest_hash++);
        if (!pAsset.isTemplateAsset())
            this.list.Add(pAsset);
        this.t.setIndexID(this.list.Count);
        this.dict.Add(id, pAsset);
        return pAsset;
    }
}

public class BaseMonoLibrary
{
    private static List<BaseMonoLibrary> list = new(); //i dont fucking know

    public static void add(BaseMonoLibrary lib)
    {
        list.Add(lib);
    }
}