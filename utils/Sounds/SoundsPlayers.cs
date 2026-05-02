using FMOD;
using NeoModLoader.services;
using NeoModLoader.utils.Collections;
using Newtonsoft.Json;
using UnityEngine;

namespace NeoModLoader.utils.Sounds;
using static FMODHelper;
/// <summary>
/// an interface that plays FMOD sounds
/// </summary>
public interface ISoundPlayer
{
    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <returns>the sound channel</returns>
    public Channel PlaySound(float pX, float pY);
}
/// <summary>
/// The Type used for which sound group this sound goes into, UI, Music, and SFX which control its volume
/// </summary>
public enum SoundType
{
    /// <summary>
    /// A part of the Music Group
    /// </summary>
    Music,
    /// <summary>
    /// A part of the Sound Group (SFX)
    /// </summary>
    Sound,
    /// <summary>
    /// A part of the UI Group
    /// </summary>
    UI
}
/// <summary>
/// The Mode which controls if and how the volume changes depending on your distance to it
/// </summary>
public enum SoundMode
{
    /// <summary>
    /// 2D sound, volume doesnt change
    /// </summary>
    Mono,
    /// <summary>
    /// 3D sound (volume changes depending on distance) which uses 2 audio channels
    /// </summary>
    Stereo,
}

public class SoundHandler
{
    public AudioChannel DrawingSound;
    public readonly List<AudioChannel> Channels = new();
    public void Update()
    {
        for (var i =0; i < Channels.Count; i++)
        {
            if (UpdateChannel(Channels[i])) continue;
            Channels.RemoveAt(i);
            i--;
        }
    }
    public void ClearChannels()
    {
        foreach (var channel in Channels)
        {
            channel.Channel.stop();
        }
        Channels.Clear();
    }
}
public class SoundAsset : Asset
{
    public readonly SlotList<ISoundPlayer> Players = new();
    public readonly SoundHandler Handler = new();
    public void AddPlayer(ISoundPlayer sound)
    {
        Players.Add(sound);
    }
    /// <summary>
    /// plays a random sound of the Players list
    /// </summary>
    /// <param name="pX">the X position</param>
    /// <param name="pY">the Y position</param>
    /// <param name="AttachedTo">The transform to attach the sound to, if 3D</param>
    /// <returns>The Audio Channel</returns>
    public AudioChannel PlaySound(float pX, float pY, Transform AttachedTo = null)
    {
        var player = GetRandom();
        AudioChannel channel = new AudioChannel(player.PlaySound(pX, pY), AttachedTo);
        Handler.Channels.Add(channel);
        return channel;
    }
    /// <summary>
    /// plays a sound at a location unless another sound with the same path is playing, then the other sound is set to that position 
    /// </summary>
    public AudioChannel PlayDrawingSound(float pX, float pY)
    {
        if(Handler.DrawingSound is { Finushed: false })
        {
            SetChannelPosition(Handler.DrawingSound.Channel, pX, pY);
        }
        else
        {
            Handler.DrawingSound = PlaySound(pX, pY);
        }
        return Handler.DrawingSound;
    }
    public SoundAsset(string id)
    {
        this.id = id;
    }
    public ISoundPlayer GetRandom()
    {
        return Players.Count == 0 ? null : Players.GetRandom();
    }
}
/// <summary>
/// a Wav Container
/// </summary>
public class SoundFilePlayer : ISoundPlayer
{
    public void WriteToFile(string path)
    {
        try
        {
            string containerjson = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, containerjson);
        }
        catch(Exception e)
        {
            LogService.LogError("Failed to write new Wav Asset because of " +e);
        }
    }
    public static bool ReadFromFile(string path, out SoundFilePlayer player)
    {
        try
        {
            player = JsonConvert.DeserializeObject<SoundFilePlayer>(File.ReadAllText(path));
            return true;
        }
        catch (Exception)
        {
            player = new SoundFilePlayer();
            return false;
        }
    }
    [JsonIgnore]public Sound Sound { get; internal set; }
    public SoundMode Mode;
    public float Volume;
    public SoundType Type;
    public int LoopCount;
    public bool Ramp;
    /// <summary>
    /// represents a Custom Sound
    /// </summary>
    /// <param name="Mode">the sound mode</param>
    /// <param name="Volume">the volume</param>
    /// <param name="LoopCount">the amount of loops</param>
    /// <param name="Ramp">if the sound is very short, this should be true</param>
    /// <param name="Type">the Type of sound, aka which group it falls into</param>
    public SoundFilePlayer(SoundMode Mode = SoundMode.Stereo, float Volume = 50, int LoopCount = 0, bool Ramp = false, SoundType Type = SoundType.Sound)
    {
        this.Ramp = Ramp;
        this.Mode = Mode;
        this.Volume = Volume;
        this.Type = Type;
        this.LoopCount = LoopCount;
    }
    /// <inheritdoc/>
    public Channel PlaySound(float pX, float pY)
    {
        Channel channel = CreateChannel(Sound, Type);
        channel.setVolumeRamp(Ramp);
        channel.setVolume(Volume/100);
        if (Mode == SoundMode.Stereo) {
            SetChannelPosition(channel, pX, pY);
        }
        return channel;
    }

    /// <summary>
    /// Generates the sound using a file
    /// </summary>
    public void GenerateSound(string FilePath)
    {
        FMODException.ThrowIfNotOk($"Unable to generate sound {FilePath}", FMODSystem.createSound(FilePath, Mode == SoundMode.Stereo ? MODE.LOOP_NORMAL | MODE._3D | MODE.CREATESTREAM : MODE.LOOP_NORMAL | MODE.CREATESTREAM, out var sound));
        sound.setLoopCount(LoopCount);
        sound.set3DMinMaxDistance(0.1f, 10000);
        Sound = sound;
    }
}