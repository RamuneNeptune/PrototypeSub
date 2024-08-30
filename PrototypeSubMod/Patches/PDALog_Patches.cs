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

        PDALog.mapping.Add("OnInterceptorTestDataDownloaded", interceptorTestEncy);
    }
}
