using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.Utility;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PowerRelay))]
public class PowerRelay_Patches
{
    [HarmonyPatch(nameof(PowerRelay.ModifyPower)), HarmonyPrefix]
    [HarmonyPatch(new[] { typeof(float), typeof(float) }, new[] { ArgumentType.Normal, ArgumentType.Out})]
    private static void ModifyPower_Prefix(PowerRelay __instance, float modified)
    {
        if (!__instance.internalPowerSource) return;
        
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModifiedPower(modified);
        }
    }
    
    [HarmonyPatch(nameof(PowerRelay.ModifyPowerFromInbound)), HarmonyPrefix]
    [HarmonyPatch(new[] { typeof(float), typeof(float) }, new[] { ArgumentType.Normal, ArgumentType.Out})]
    private static void ModifyPowerFromInbound_Prefix(PowerRelay __instance, float modified)
    {
        if (__instance.gameObject.TryGetComponent(out OnModifyPowerEvent powerEvent))
        {
            powerEvent.ModifiedPower(modified);
        }
    }
}