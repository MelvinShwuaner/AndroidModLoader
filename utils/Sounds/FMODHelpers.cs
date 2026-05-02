using FMOD;
using FMODUnity;
using HarmonyLib;
using UnityEngine;

namespace NeoModLoader.utils.Sounds;
using static SoundLibrary;
using static FMODHelper;
using static FMODException;

internal static class FMODPatches
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MusicBox), nameof(MusicBox.playDrawingSound))]
    [HarmonyPriority(Priority.Last)]
    static bool PlayDrawingSoundPatch(string pSoundPath, float pX, float pY)
    {
        if (!MusicBox.sounds_on)
        {
            return true;
        }
        if (!MainLibrary.dict.TryGetValue(pSoundPath, out var player)) return true;
        player.PlayDrawingSound(pX, pY);
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MusicBox), nameof(MusicBox.playSound), typeof(string), typeof(float), typeof(float),
        typeof(bool), typeof(bool))]
    [HarmonyPriority(Priority.Last)]
    static bool PlaySoundPatch(string pSoundPath, float pX, float pY, bool pGameViewOnly)
    {
        if (!MusicBox.sounds_on)
        {
            return true;
        }
        if (pGameViewOnly && World.world.quality_changer.isLowRes())
        {
            return true;
        }
        if (!MainLibrary.dict.TryGetValue(pSoundPath, out var player)) return true;
        player.PlaySound(pX, pY);
        return false;
    }
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapBox), nameof(MapBox.clearWorld))]
    static void ClearAllCustomSounds()
    {
       MainLibrary.ClearSounds();
    }
    [HarmonyPostfix]
    [HarmonyPatch(typeof(RuntimeManager), "Update")]
    static void Update()
    {
        SFXGroup.setVolume(GetVolume(SoundType.Sound));
        MusicGroup.setVolume(GetVolume(SoundType.Music));
        UIGroup.setVolume(GetVolume(SoundType.UI));
        MainLibrary.Update();
    }
}
/// <summary>
/// a helper class for FMOD
/// </summary>
public static class FMODHelper
{
    public static ChannelGroup SFXGroup { get; private set; }
    public static ChannelGroup MusicGroup { get; private set; }
    public static ChannelGroup UIGroup { get; private set; }
    public static FMOD.System FMODSystem { get; private set; }

    internal static void InitFMOD()
    {
        ThrowIfNotOk("Failed to initialize FMOD Core System!", RuntimeManager.StudioSystem.getCoreSystem(out var coresystem));
        FMODSystem = coresystem;
        ThrowIfNotOk("Failed to create SFXGroup", FMODSystem.createChannelGroup("SFXGroup", out var Group));
        SFXGroup = Group;
        ThrowIfNotOk("Failed to create MusicGroup", FMODSystem.createChannelGroup("MusicGroup", out Group)); 
        MusicGroup = Group;
        ThrowIfNotOk("Failed to create UIGroup", FMODSystem.createChannelGroup("UIGroup", out Group)); 
        UIGroup = Group;
    }
    public static float GetVolume(SoundType soundType)
    {
        float Volume = soundType switch
        {
            SoundType.Music => PlayerConfig.getIntValue("volume_music"),
            SoundType.Sound => PlayerConfig.getIntValue("volume_sound_effects"),
            _ => PlayerConfig.getIntValue("volume_ui")
        } * PlayerConfig.getIntValue("volume_master_sound") / 10000f;
        return Volume;
    }
    /// <summary>
    /// Sets the position of a channel, channel must be Stereo
    /// </summary>
    public static void SetChannelPosition(Channel channel, float pX, float pY)
    {
        channel.get3DAttributes(out VECTOR Pos, out VECTOR vel);
        if (Pos.x != pX || Pos.y != pY)
        {
            VECTOR pos = new() { x = pX, y = pY, z = 0 };
            channel.set3DAttributes(ref pos, ref vel);
        }
    }
    /// <summary>
    /// Updates a Audio Channel
    /// </summary>
    public static bool UpdateChannel(AudioChannel audioChannel)
    {
        if (audioChannel.Finushed)
        {
            return false;
        }
        if (audioChannel.AttachedTo != null)
        {
            SetChannelPosition(audioChannel.Channel, audioChannel.AttachedTo.position.x, audioChannel.AttachedTo.position.y);
        }
        return true;
    }
    /// <summary>
    /// creates a new FMOD Channel with a sound and type
    /// </summary>
    /// <exception cref="FMODException">if it fails to play the sound for whatever reason</exception>
    public static Channel CreateChannel(Sound Sound, SoundType Type)
    {
        Channel channel;
        ThrowIfNotOk($"Failed to Play Sound", Type switch
        {
            SoundType.Music => FMODSystem.playSound(Sound, MusicGroup, false, out channel),
            SoundType.UI => FMODSystem.playSound(Sound, UIGroup, false, out channel),
            _ =>  FMODSystem.playSound(Sound, SFXGroup, false, out channel),
        });
        return channel;
    }
}
/// <summary>
/// represents a failed <see cref="RESULT"/>
/// </summary>
public class FMODException : Exception
{
    /// <inheritdoc/>
    public FMODException(string msg) : base(msg)
    {
    }
    /// <summary>
    /// throws an FMOD Exception if the result is not <see cref="RESULT.OK"/>
    /// </summary>
    public static void ThrowIfNotOk(string Msg, RESULT result)
    {
        if (result != RESULT.OK)
        {
            throw new FMODException(Msg + " due to " + result);
        }
    }
}
/// <summary>
/// A container to manage the Sound
/// </summary>
public class AudioChannel
{
    /// <summary>
    /// A FMOD Audio Channel which plays the sound
    /// </summary>
    public readonly Channel Channel;
    /// <summary>
    /// The Transform or gameobject which this Sound is attached two. sounds whose mode is BASIC must not use this
    /// </summary>
    public Transform AttachedTo;
    internal AudioChannel(Channel channel, Transform attachedTo = null) : this()
    {
        Channel = channel;
        AttachedTo = attachedTo;
    }
    private AudioChannel(){}
    /// <summary>
    /// returns true if the channel has stopped playing or something wrong happened
    /// </summary>
    public bool Finushed => Channel.isPlaying(out bool IsPlaying) != RESULT.OK || !IsPlaying;
}