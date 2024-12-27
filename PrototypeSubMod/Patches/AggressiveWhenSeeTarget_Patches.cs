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

        __result = RedirectPlayerTarget(__result);
    }

    public static GameObject RedirectPlayerTarget(GameObject player)
    {
        bool overrideResult = false;
        CloakEffectHandler currentHandler = null;

        foreach (var handler in CloakEffectHandler.EffectHandlers)
        {
            if (handler.GetAllowedToCloak() && handler.IsInsideOvoid(player.transform.position))
            {
                overrideResult = true;
                currentHandler = handler;
                break;
            }
        }

        if (!overrideResult) return player;

        Player_Patches.DummyLDFTarget.transform.position = currentHandler.GetClosestPointOnSurface(player.transform.position, 1.1f);
        return Player_Patches.DummyLDFTarget;
    }
}
