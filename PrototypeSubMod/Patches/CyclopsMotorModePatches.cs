using HarmonyLib;
using PrototypeSubMod.IonGenerator;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsMotorMode))]
internal class CyclopsMotorModePatches
{
    [HarmonyPatch(nameof(CyclopsMotorMode.GetNoiseValue)), HarmonyPostfix]
    private static void GetNoiseValue_Postfix(CyclopsMotorMode __instance, ref float __result)
    {
        var ionGenerator = __instance.GetComponentInChildren<ProtoIonGenerator>();
        if (!ionGenerator) return;

        __result = ionGenerator.GetNoiseValue();
    }
}
