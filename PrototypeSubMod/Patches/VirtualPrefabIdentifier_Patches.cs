using HarmonyLib;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(VirtualPrefabIdentifier))]
public class VirtualPrefabIdentifier_Patches
{
    private static readonly Vector3 MOONPOOL_DOOR_POS = new(781, -323, -1050);
    
    [HarmonyPatch(nameof(VirtualPrefabIdentifier.Start)), HarmonyPrefix]
    private static bool Start_Prefix(VirtualPrefabIdentifier __instance)
    {
        if ((__instance.transform.position - MOONPOOL_DOOR_POS).sqrMagnitude < 10000)
        {
            GameObject.Destroy(__instance.gameObject);
            return false;
        }

        return true;
    }
}