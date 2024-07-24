using HarmonyLib;
using PrototypeSubMod.Monobehaviors;
using System.Linq;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Equipment))]
internal class EquipmentPatches
{
    [HarmonyPatch(nameof(Equipment.AllowedToAdd)), HarmonyPostfix]
    private static void AllowedToAdd_Postfix(Equipment __instance, Pickupable pickupable, ref bool __result)
    {
        Plugin.Logger.LogInfo($"Transform = {__instance.tr}");

        if (!__instance.tr.parent) return;

        if (!__instance.tr.parent.TryGetComponent(out PrototypePowerSystem powerSystem)) return;

        __result = powerSystem.GetAllowedTechTypes().Contains(pickupable.GetTechType());
    }
}
