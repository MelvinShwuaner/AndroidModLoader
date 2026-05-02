namespace NeoModLoader.utils.Sounds;
/// <summary>
/// an Audio library. multiple can be used
/// </summary>
public class SoundLibrary : AssetLibrary<SoundAsset>
{
    /// <summary>
    /// the main audio library. updates by itself
    /// </summary>
    public static SoundLibrary MainLibrary { get; internal set; }
    public void ClearSounds()
    {
        foreach (var sound in list)
        {
           sound.Handler.ClearChannels();
        }
    }
    public void Update()
    {
        foreach (var sound in list)
        {
            sound.Handler.Update();
        }
    }
    public SoundAsset GetOrCreate(string ID)
    {
        return dict.TryGetValue(ID, out var checkID) ? checkID : add(new SoundAsset(ID));
    }
}