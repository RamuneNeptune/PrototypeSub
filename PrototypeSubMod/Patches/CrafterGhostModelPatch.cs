using System.Collections;
using HarmonyLib;
using PrototypeSubMod.Prefabs;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CrafterGhostModel))]
public class CrafterGhostModelPatch
{

    //Fixes an issue where the construction VFX of an item inside the fabricator does not complete, even after the item is built.
    [HarmonyPatch(nameof(CrafterGhostModel.SetupGhostModelAsync))]
    [HarmonyPostfix]
    public static IEnumerator UpdateProgress_Postfix(IEnumerator result, CrafterGhostModel __instance)
    {
        yield return result;

        if (__instance.boundsToVFX != null && __instance.GetComponentInChildren<AlienFabricator>() != null)
        {
            __instance.boundsToVFX.localMinY = -0.4f;
            __instance.boundsToVFX.localMaxY = 0.4f;
        }
    }
    
}