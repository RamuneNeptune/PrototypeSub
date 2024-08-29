using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace PrototypeSubMod;

internal static class PrototypeAudio
{
    public const MODE k3DSoundModes = MODE.DEFAULT | MODE._3D | MODE.ACCURATETIME | MODE._3D_LINEARSQUAREROLLOFF;
    public const MODE k2DSoundModes = MODE.DEFAULT | MODE._2D | MODE.ACCURATETIME;
    public const MODE kStreamSoundModes = k2DSoundModes | MODE.CREATESTREAM;
    public const MODE kWorldSoundModes = k3DSoundModes | MODE.CREATESTREAM;

    internal static void RegisterAudio(AssetBundle bundle)
    {
        RegisterWorldSFX(bundle);
        RegisterVoicelines(bundle);
    }

    private static void RegisterWorldSFX(AssetBundle bundle)
    {
        AddSFX(bundle.LoadAsset<AudioClip>("ShortRangeTeleporterSFX"), "InterfloorTeleporterFX", kWorldSoundModes);
    }

    private static void RegisterVoicelines(AssetBundle bundle)
    {
        AddSFX(bundle.LoadAsset<AudioClip>("PDA_InterceptorUnlock"), "PDA_InterceptorUnlock", kStreamSoundModes);
    }

    private static void AddSFX(AudioClip clip, string soundPath, MODE modes)
    {
        var sound = AudioUtils.CreateSound(clip, modes);
        CustomSoundHandler.RegisterCustomSound(soundPath, sound, AudioUtils.BusPaths.VoiceOvers);
    }
}
