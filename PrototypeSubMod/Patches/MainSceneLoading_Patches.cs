using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MainSceneLoading))]
public class MainSceneLoading_Patches
{
    [HarmonyPatch(nameof(MainSceneLoading.Launch)), HarmonyPrefix]
    private static void Launch_Prefix()
    {
        Plugin.prefabLoadWaitItem = WaitScreen.Add("ProtoEasyPrefabs");
    }
}