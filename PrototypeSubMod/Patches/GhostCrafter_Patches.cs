using HarmonyLib;
using PrototypeSubMod.Prefabs;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(GhostCrafter))]
public class GhostCrafter_Patches
{

    [HarmonyPatch(nameof(GhostCrafter.HasEnoughPower))]
    [HarmonyPrefix]
    private static bool HasEnoughPower_Prefix(GhostCrafter __instance, ref bool __result)
    {
        if (__instance.gameObject.TryGetComponent(typeof(AlienFabricator), out _))
        {
            __result = true;
            return false;
        }
        
        __result = false;
        return true;
    }
    
}