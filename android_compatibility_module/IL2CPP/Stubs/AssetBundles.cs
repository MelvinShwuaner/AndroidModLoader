namespace UnityEngine;

public class AssetBundle //funny
{
    public string[] GetAllAssetNames() => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public string[] GetAllScenePaths() => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public T LoadAsset<T>(params object[] param) => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public Object LoadAsset(params object[] param) => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public Object[] LoadAllAssets(params object[] param) => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public T[] LoadAllAssets<T>(params object[] param) => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public static AssetBundle LoadFromStream(Stream stream) => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
    public string name => throw new PlatformNotSupportedException("Asset Bundles are not supported on android");
}