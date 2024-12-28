using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(AggressiveWhenSeeTarget))]
internal class AggressiveWhenSeeTarget_Patches
{
    [HarmonyPatch(nameof(AggressiveWhenSeeTarget.GetAggressionTarget)), HarmonyPostfix]
    private static void GetAggressionTarget_Postfix(ref GameObject __result)
    {
        if (__result != Player.main.gameObject) return;

        __result = RedirectAggroTarget(__result);
    }

    public static GameObject RedirectAggroTarget(GameObject target)
    {
        bool overrideResult = false;
        CloakEffectHandler currentHandler = null;

        foreach (var handler in CloakEffectHandler.EffectHandlers)
        {
            if (handler.GetAllowedToCloak() && handler.IsInsideOvoid(target.transform.position))
            {
                overrideResult = true;
                currentHandler = handler;
                break;
            }
        }

        if (!overrideResult) return target;

        Player_Patches.DummyLDFTarget.transform.position = currentHandler.GetClosestPointOnSurface(target.transform.position, 1.1f);
        return Player_Patches.DummyLDFTarget;
    }
}
