using HarmonyLib;
using PrototypeSubMod.RepairBots;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsExternalDamageManager))]
internal class DamageManagerPatches
{
    [HarmonyPatch(nameof(CyclopsExternalDamageManager.CreatePoint)), HarmonyPostfix]
    private static void CreatePoint_Postfix(CyclopsExternalDamageManager __instance)
    {
        if (!__instance.TryGetComponent(out RepairPointManager pointManager)) return;

        pointManager.OnDamagePointsChanged();
    }

    [HarmonyPatch(nameof(CyclopsExternalDamageManager.RepairPoint)), HarmonyPostfix]
    private static void RepairPoint_Postfix(CyclopsExternalDamageManager __instance)
    {
        if (!__instance.TryGetComponent(out RepairPointManager pointManager)) return;

        pointManager.OnDamagePointsChanged();
    }
}
