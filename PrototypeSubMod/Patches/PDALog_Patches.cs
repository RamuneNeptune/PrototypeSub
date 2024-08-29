using HarmonyLib;
using Nautilus.Utility;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PDALog))]
internal class PDALog_Patches
{
    private static Sprite pdaSprite;

    [HarmonyPatch(nameof(PDALog.Initialize)), HarmonyPostfix]
    private static void Initialize_Postfix()
    {
        pdaSprite = PDALog.mapping.First(i => i.Value.key == "Goal_BiomeKooshZone").Value.icon;

        AddEntries();
    }

    [HarmonyPatch(nameof(PDALog.Add)), HarmonyPostfix]
    private static void Add_Postfix(string key)
    {
        PDALog.GetEntryData(key, out var data);
        Plugin.Logger.LogInfo($"Adding entry data with key {key}. Data key = {data.key} | Sound = {data.sound} (Null = {data.sound == null}) | Length = {(float)FMODExtensions.GetLength(data.sound.path)}");
    }

    private static void AddEntries()
    {
        var fmodAsset = AudioUtils.GetFmodAsset("PDA_InterceptorUnlock");
        fmodAsset.id = fmodAsset.path;

        PDALog.EntryData interceptorTestEncy = new()
        {
            key = "OnInterceptorTestDataDownloaded",
            type = PDALog.EntryType.Default,
            icon = pdaSprite,
            sound = fmodAsset,
            doNotAutoPlay = false
        };

        Plugin.Logger.LogInfo($"Adding entry to {PDALog.mapping} with key \"OnInterceptorTestDataDownloaded\" | FMOD Asset = {interceptorTestEncy.sound.id}");
        PDALog.mapping.Add("OnInterceptorTestDataDownloaded", interceptorTestEncy);

        PDALog.GetEntryData("OnInterceptorTestDataDownloaded", out var data);
        Plugin.Logger.LogInfo($"Sound after add = {data.sound.path} | Key = {data.key}");
    }
}
