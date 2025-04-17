using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using PrototypeSubMod.Prefabs;
using UnityEngine;

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

        if (__instance.GetComponentInChildren<AlienFabricator>() != null)
        {
            switch (techType)
            {
                case TechType.PrecursorIonBattery:
                    __instance.ghostModel.transform.localPosition = new Vector3(0f, 0f, 0.07f);
                    __instance.ghostModel.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                    __instance.ghostModel.transform.localScale *= 0.9f;
                    break;
                case TechType.PrecursorIonPowerCell:
                    __instance.ghostModel.transform.localPosition = new Vector3(0f, 0f, 0.03f);
                    __instance.ghostModel.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                    __instance.ghostModel.transform.localScale *= 0.6f;
                    break;
            }
            
            if (__instance.boundsToVFX != null)
            {
                __instance.boundsToVFX.localMinY = -0.16f;
                __instance.boundsToVFX.localMaxY = 0.3f;
            }
        }
    }
    
}