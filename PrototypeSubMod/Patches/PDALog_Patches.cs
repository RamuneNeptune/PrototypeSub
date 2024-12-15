using HarmonyLib;
using Nautilus.Utility;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PDALog))]
internal class PDALog_Patches
{
    public static List<(string assetName, string key)> entries = new();

    private static Sprite pdaSprite;

    [HarmonyPatch(nameof(PDALog.Initialize)), HarmonyPostfix]
    private static void Initialize_Postfix()
    {
        pdaSprite = PDALog.mapping.First(i => i.Value.key == "Goal_BiomeKooshZone").Value.icon;

        AddEntries();
    }

    private static void AddEntries()
    {
        foreach (var item in entries)
        {
            var fmodAsset = AudioUtils.GetFmodAsset(item.assetName);
            fmodAsset.id = fmodAsset.path;

            PDALog.EntryData interceptorTestEncy = new()
            {
                key = item.key,
                type = PDALog.EntryType.Default,
                icon = pdaSprite,
                sound = fmodAsset,
                doNotAutoPlay = false
            };

            PDALog.mapping.Add(item.key, interceptorTestEncy);
        }
    }
}
