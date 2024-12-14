using HarmonyLib;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(EntitySlot))]
internal class EntitySlotPatches
{
    private readonly static Bounds MoonpoolBounds = new Bounds(new Vector3(790.5f, -317.5f, -1047.5f), new Vector3(73, 15, 71));

    [HarmonyPatch(nameof(EntitySlot.Start)), HarmonyPrefix]
    private static bool Start_Prefix(EntitySlot __instance)
    {
        bool inBounds = MoonpoolBounds.Contains(__instance.transform.position);
        if (!inBounds) return true;

        __instance.beingDestroyedAfterUse = true;
        GameObject.Destroy(__instance.gameObject);
        return false;
    }
}
