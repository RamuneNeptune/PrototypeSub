using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PDALog))]
internal class PDALog_Patches
{
    private static Sprite pdaSprite;

    [HarmonyPatch(nameof(PDALog.Initialize))]
    private static void Initialize_Postfix()
    {
        pdaSprite = PDALog.mapping.First(i => i.Value.key == "Goal_BiomeKooshZone").Value.icon;

        AddEntries();
    }

    private static void AddEntries()
    {
        /*
        PDALog.EntryData interceptorTestEncy = new()
        {
            key = "",
            type = PDALog.EntryType.Default,
            icon = pdaSprite,
            sound = Plugin.AssetBundle.LoadAsset<FMODAsset>("PDAInterceptorUnlock"),
            doNotAutoPlay = false
        };
        PDALog.mapping.Add()
        */
    }
}
