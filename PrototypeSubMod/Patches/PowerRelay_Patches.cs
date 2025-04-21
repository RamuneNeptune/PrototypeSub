using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Utility;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PowerRelay))]
public class PowerRelay_Patches
{
    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPostfix]
    private static void ModifyPower_Postfix(PowerRelay __instance, float amount)
    {
        if (!__instance.internalPowerSource) return;
        
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModiedPower(amount);
        }
    }
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPowerFromInbound)), HarmonyPostfix]
    private static void ModifyPowerFromInbound_Postfix(PowerRelay __instance, float amount)
    {
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModiedPower(amount);
        }
    }
}