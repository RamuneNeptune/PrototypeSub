using HarmonyLib;
using PrototypeSubMod.Docking;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PlayerCinematicController))]
public class PlayerCinematicController_Patches
{
    [HarmonyPatch(nameof(PlayerCinematicController.StartCinematicMode)), HarmonyPrefix]
    private static bool StartCinematicMode_Prefix(PlayerCinematicController __instance)
    {
        if (!__instance.TryGetComponent(out IgnoreCinematicStart ignoreCinematicStart)) return true;
        
        return !ignoreCinematicStart.enabled;
    }
}