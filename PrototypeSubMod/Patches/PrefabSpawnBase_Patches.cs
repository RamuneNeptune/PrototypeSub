using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors.PrefabRetrievers;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PrefabSpawnBase))]
public class PrefabSpawnBase_Patches
{
    [HarmonyPatch(nameof(PrefabSpawnBase.Awake)), HarmonyPrefix]
    private static bool Awake_Prefix(PrefabSpawnBase __instance)
    {
        if (!__instance.HasValidPrefab() && __instance.TryGetComponent(out FirePrefabSetter _)) return false;

        return true;
    }
    
    [HarmonyPatch(nameof(PrefabSpawnBase.Start)), HarmonyPrefix]
    private static bool Start_Prefix(PrefabSpawnBase __instance)
    {
        if (!__instance.HasValidPrefab() && __instance.TryGetComponent(out FirePrefabSetter _)) return false;

        return true;
    }
}