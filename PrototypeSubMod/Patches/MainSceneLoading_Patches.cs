using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MainSceneLoading))]
public class MainSceneLoading_Patches
{
    [HarmonyPatch(nameof(MainSceneLoading.Launch)), HarmonyPrefix]
    private static void Launch_Prefix()
    {
        if (Plugin.easyPrefabsLoaded) return;
        
        Plugin.prefabLoadWaitItem = WaitScreen.Add("ProtoEasyPrefabs");
    }
}