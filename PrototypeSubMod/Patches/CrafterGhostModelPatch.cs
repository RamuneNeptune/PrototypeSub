using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using PrototypeSubMod.Prefabs;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CrafterGhostModel))]
public class CrafterGhostModelPatch
{
    
    private static Dictionary<TechType, TechType> mappedTechTypes = new()
    {
        { TechType.PrecursorIonCrystal, PrecursorIonCrystal_Craftable.craftableCrystalInfo.TechType },
        { TechType.PrecursorIonCrystalMatrix, CrystalMatrix_Craftable.craftableCrystalMatrixInfo.TechType }
    };

    [HarmonyPatch(nameof(CrafterGhostModel.SetupGhostModelAsync))]
    [HarmonyPostfix]
    public static IEnumerator SetupGhostModelAsync_Postfix(IEnumerator result, CrafterGhostModel __instance, TechType techType)
    {
        if (mappedTechTypes.TryGetValue(techType, out var replacementTechType))
        {
            yield return __instance.SetupGhostModelAsync(replacementTechType);
            
            yield break;
        }
        
        yield return result;

        if (__instance.boundsToVFX != null && __instance.GetComponentInChildren<AlienFabricator>() != null)
        {
            __instance.boundsToVFX.localMinY = -0.4f;
            __instance.boundsToVFX.localMaxY = 0.4f;
        }
    }
    
}