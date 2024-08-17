using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(AggressiveWhenSeePlayer))]
internal class AggressiveWhenSeePlayerPatches
{
    [HarmonyPatch(nameof(AggressiveWhenSeePlayer.GetAggressionTarget)), HarmonyPostfix]
    private static void GetAggressionTarget_Postfix(ref GameObject __result)
    {
        if (!CloakEffectHandler.Instance) return;

        if (CloakEffectHandler.Instance.IsInsideOvoid(Camera.main.transform.position) && CloakEffectHandler.Instance.effectEnabled)
        {
            __result = null;
        }
    }
}
