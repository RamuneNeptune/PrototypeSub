using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Utility;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PowerRelay))]
public class PowerRelay_Patches
{
    [SaveStateReference(false)]
    private static bool modifyingPower;
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPrefix]
    private static void ModifyPower_Prefix(PowerRelay __instance)
    {
        modifyingPower = true;
    }
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPostfix]
    private static void ModifyPower_Postfix(PowerRelay __instance, float amount)
    {
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModiedPower(amount);
        }
        
        modifyingPower = false;
    }
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPowerFromInbound)), HarmonyPostfix]
    private static void ModifyPowerFromInbound_Postfix(PowerRelay __instance, float amount)
    {
        if (modifyingPower) return;
        
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModiedPower(amount);
        }
    }
}