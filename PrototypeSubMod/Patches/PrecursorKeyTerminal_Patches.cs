using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PrecursorKeyTerminal))]
internal class PrecursorKeyTerminal_Patches
{
    [HarmonyPatch(nameof(PrecursorKeyTerminal.Start)), HarmonyPrefix]
    private static bool Start_Prefix(PrecursorKeyTerminal __instance)
    {
        return (int)__instance.acceptKeyType < __instance.keyMats.Length;
    }
}
