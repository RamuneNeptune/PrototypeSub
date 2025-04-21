using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Utility;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PowerRelay))]
public class PowerRelay_Patches
{
    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPrefix]
    private static void ModifyPower_Prefix(PowerRelay __instance, float amount)
    {
        if (!__instance.internalPowerSource) return;
        
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModifiedPower(amount);
        }
    }
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPowerFromInbound)), HarmonyPrefix]
    private static void ModifyPowerFromInbound_Prefix(PowerRelay __instance, float amount)
    {
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModifiedPower(amount);
        }
    }
}