using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.SubSystems;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CullingOccluder))]
public class CullingOccluder_Patches
{
    [HarmonyPatch(nameof(CullingOccluder.Start)), HarmonyPostfix]
    private static void Start_Postfix(CullingOccluder __instance)
    {
        if (__instance.TryGetComponent(out DisableRendAfterBoundsCached _))
        {
            __instance.occluderRenderer.enabled = false;
        }
    }
}