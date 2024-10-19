using HarmonyLib;
using PrototypeSubMod.Teleporter;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PrecursorTeleporterActivationTerminal))]
internal class PrecursorTeleporterActivationTerminalPatches
{
    [HarmonyPatch(nameof(PrecursorTeleporterActivationTerminal.OnProxyHandClick)), HarmonyPostfix]
    private static void OnProxyHandClick_Postfix(PrecursorTeleporterActivationTerminal __instance)
    {
        if (!__instance.unlocked) return;

        var manager = __instance.GetComponentInParent<ProtoTeleporterManager>();
        if (!manager) return;

        manager.OnActivationTerminalCinematicStarted();
    }

    [HarmonyPatch(nameof(PrecursorTeleporterActivationTerminal.OnPlayerCinematicModeEnd)), HarmonyPostfix]
    private static void OnPlayerCinematicModeEnd_Postfix(PrecursorTeleporterActivationTerminal __instance)
    {
        var manager = __instance.GetComponentInParent<ProtoTeleporterManager>();
        if (!manager) return;

        manager.OnActivationTerminalCinematicEnded();
    }
}
