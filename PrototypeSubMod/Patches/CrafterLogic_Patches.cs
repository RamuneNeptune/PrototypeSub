using HarmonyLib;
using PrototypeSubMod.Prefabs;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CrafterLogic))]
public class CrafterLogic_Patches
{

    [HarmonyPatch(nameof(CrafterLogic.ConsumeEnergy))]
    [HarmonyPrefix]
    public static bool ConsumeEnergy_Prefix(CrafterLogic __instance, PowerRelay powerRelay, ref bool __result)
    {
        if (powerRelay != null && powerRelay.gameObject.TryGetComponent(out AlienFabricator _))
        {
            __result = true;
            return false;
        }
        
        return true;
    }
    
}