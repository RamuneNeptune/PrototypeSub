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
    public static List<(string assetName, string key)> orionEntries = new();

    [HarmonyPatch(nameof(PDALog.Initialize)), HarmonyPostfix]
    private static void Initialize_Postfix()
    {
        var pdaSprite = PDALog.mapping.First(i => i.Value.key == "Goal_BiomeKooshZone").Value.icon;

        AddEntries(entries, pdaSprite);
        AddEntries(orionEntries, Plugin.AssetBundle.LoadAsset<Sprite>("ProtoIcon"));
    }

    private static void AddEntries(List<(string assetName, string key)> entriesToRegister, Sprite sprite)
    {
        foreach (var item in entriesToRegister)
        {
            var fmodAsset = AudioUtils.GetFmodAsset(item.assetName);
            fmodAsset.id = fmodAsset.path;

            PDALog.EntryData ency = new()
            {
                key = item.key,
                type = PDALog.EntryType.Default,
                icon = sprite,
                sound = fmodAsset,
                doNotAutoPlay = false
            };

            PDALog.mapping.Add(item.key, ency);
        }
    }
}
