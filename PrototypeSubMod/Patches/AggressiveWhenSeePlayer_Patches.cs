using HarmonyLib;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(AggressiveWhenSeePlayer))]
internal class AggressiveWhenSeePlayer_Patches
{
    [HarmonyPatch(nameof(AggressiveWhenSeePlayer.GetAggressionTarget)), HarmonyPostfix]
    private static void GetAggressionTarget(AggressiveWhenSeePlayer __instance, ref GameObject __result)
    {
        if (__result == Player.main.gameObject)
        {
            __result = AggressiveWhenSeeTarget_Patches.RedirectAggroTarget(__result);
        }
    }
}
