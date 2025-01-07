using HarmonyLib;
using PrototypeSubMod.IonBarrier;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(DamageSystem))]
internal class DamageSystem_Patches
{
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage)), HarmonyPostfix]
    private static void CalculateDamage_Postfix(ref float __result, DamageType type, GameObject target)
    {
        var ionBarrier = target.GetComponentInChildren<ProtoIonBarrier>(true);
        if (ionBarrier == null) return;

        if (!ionBarrier.GetUpgradeEnabled() || !ionBarrier.GetUpgradeInstalled()) return;

        __result *= ionBarrier.GetReductionForType(type);
    }
}
