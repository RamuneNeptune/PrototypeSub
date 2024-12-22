using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(AggressiveWhenSeePlayer))]
internal class AggressiveWhenSeePlayer_Patches
{
    [HarmonyPatch(nameof(AggressiveWhenSeePlayer.GetAggressionTarget)), HarmonyPostfix]
    private static void GetAggressionTarget_Postfix(ref GameObject __result)
    {
        if (Player.main.currentSub) return;

        var cloakHandler = Player.main.currentSub.GetComponentInChildren<CloakEffectHandler>();
        if (cloakHandler == null) return;

        if (cloakHandler.IsInsideOvoid(Camera.main.transform.position) && cloakHandler.GetAllowedToCloak())
        {
            __result = null;
        }
    }
}
