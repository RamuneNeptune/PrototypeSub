using HarmonyLib;
using PrototypeSubMod.IonBarrier;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(DamageSystem))]
internal class DamageSystem_Patches
{
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage)), HarmonyPrefix]
    private static void CalculateDamage_Prefix(ref float damage, DamageType type, GameObject target)
    {
        var ionBarrier = target.GetComponentInChildren<ProtoIonBarrier>(true);
        if (ionBarrier == null) return;

        if (!ionBarrier.GetUpgradeEnabled() || !ionBarrier.GetUpgradeInstalled()) return;

        damage *= ionBarrier.GetReductionForType(type);
    }
}
