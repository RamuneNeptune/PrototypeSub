using HarmonyLib;
using PrototypeSubMod.MotorHandler;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsMotorMode))]
internal class CyclopsMotorMode_Patches
{
    [HarmonyPatch(nameof(CyclopsMotorMode.GetNoiseValue)), HarmonyPostfix]
    private static void GetNoiseValue_Postfix(CyclopsMotorMode __instance, ref float __result)
    {
        var motorHandler = __instance.GetComponentInChildren<ProtoMotorHandler>();
        if (!motorHandler) return;

        float overrideValue = motorHandler.GetOverrideNoiseValue();
        if (overrideValue < 0) return;

        __result = overrideValue;
    }
}
