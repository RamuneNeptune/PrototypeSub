using HarmonyLib;
using PrototypeSubMod.Prefabs.AlienBuildingBlock;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Warper))]
public class WarperPatch
{
    
    [HarmonyPatch(nameof(Warper.EndWarpOut))]
    [HarmonyPrefix]
    public static void EndWarpOut_Prefix(Warper __instance)
    {
        if (Random.Range(0, 5) == 4)
        {
            CoroutineHost.StartCoroutine(InactiveAlienBuildingBlock.TrySpawnBiome(__instance.transform.position,
                LargeWorld.main.GetBiome(__instance.transform.position)));
        }
    }
    
}