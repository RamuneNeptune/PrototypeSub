using HarmonyLib;
using PrototypeSubMod.Teleporter;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PrecursorTeleporterActivationTerminal))]
internal class PrecursorTeleporterActivationTerminal_Patches
{
    [HarmonyPatch(nameof(PrecursorTeleporterActivationTerminal.OnProxyHandClick)), HarmonyPrefix]
    private static bool OnProxyHandClick_Prefix(PrecursorTeleporterActivationTerminal __instance)
    {
        var keyTrigger = __instance.GetComponent<ProtoKeyTerminalTrigger>();
        if (!keyTrigger) return true;

        if (!keyTrigger.GetIsLocked()) return true;

        keyTrigger.OnClickDenied();
        return false;
    }

    [HarmonyPatch(nameof(PrecursorTeleporterActivationTerminal.OnProxyHandClick)), HarmonyPostfix]
    private static void OnProxyHandClick_Postfix(PrecursorTeleporterActivationTerminal __instance)
    {
        if (!__instance.unlocked) return;

        var manager = __instance.GetComponentInParent<ProtoTeleporterManager>();
        if (!manager) return;

        manager.OnActivationTerminalCinematicStarted();
        __instance.OpenDeck();
    }

    [HarmonyPatch(nameof(PrecursorTeleporterActivationTerminal.OnPlayerCinematicModeEnd)), HarmonyPostfix]
    private static void OnPlayerCinematicModeEnd_Postfix(PrecursorTeleporterActivationTerminal __instance)
    {
        var manager = __instance.GetComponentInParent<ProtoTeleporterManager>();
        if (!manager) return;

        manager.OnActivationTerminalCinematicEnded();
    }
}
