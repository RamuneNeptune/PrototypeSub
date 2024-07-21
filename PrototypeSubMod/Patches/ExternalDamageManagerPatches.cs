using HarmonyLib;
using PrototypeSubMod.Prefabs;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsExternalDamageManager))]
internal class ExternalDamageManagerPatches
{
    [HarmonyPatch(nameof(CyclopsExternalDamageManager.OnEnable)), HarmonyPrefix]
    private static bool OnEnable_Prefix(CyclopsExternalDamageManager __instance)
    {
        var techTag = __instance.subRoot.GetComponent<TechTag>();
        if (techTag && techTag.type == Prototype_Craftable.SubInfo.TechType) return false;

        return true;
    }
}
