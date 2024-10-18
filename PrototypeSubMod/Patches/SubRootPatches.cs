using HarmonyLib;
using PrototypeSubMod.MotorHandler;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(SubRoot))]
internal class SubRootPatches
{
    [HarmonyPatch(nameof(SubRoot.UpdatePowerRating)), HarmonyPostfix]
    private static void UpdatePowerRating_Postfix(SubRoot __instance)
    {
        var motorHandler = __instance.GetComponentInChildren<ProtoMotorHandler>(true);
        if (!motorHandler) return;

        __instance.currPowerRating *= motorHandler.GetEfficiencyMultiplier();
    }
}
