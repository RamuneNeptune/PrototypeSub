using HarmonyLib;
using PrototypeSubMod.MotorHandler;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsMotorMode))]
internal class CyclopsMotorModePatches
{
    [HarmonyPatch(nameof(CyclopsMotorMode.GetNoiseValue)), HarmonyPostfix]
    private static void GetNoiseValue_Postfix(CyclopsMotorMode __instance, ref float __result)
    {
        var motorHandler = __instance.GetComponentInChildren<ProtoMotorHandler>();
        if (!motorHandler) return;

        __result = motorHandler.GetOverrideNoiseValue();
    }
}
