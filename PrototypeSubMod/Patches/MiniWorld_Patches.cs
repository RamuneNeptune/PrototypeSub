using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MiniWorld))]
public class MiniWorld_Patches
{
    [HarmonyPatch(nameof(MiniWorld.Start)), HarmonyPrefix]
    private static bool Start_Prefix(MiniWorld __instance)
    {
        return __instance.materialInstance == null;
    }
}