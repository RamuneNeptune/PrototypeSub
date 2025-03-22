using HarmonyLib;
using PrototypeSubMod.PrototypeStory;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(SceneCleaner))]
public class SceneCleaner_Patches
{
    [HarmonyPatch(nameof(SceneCleaner.Start)), HarmonyPrefix]
    private static void Start_Prefix(SceneCleaner __instance)
    {
        if (!EndCinematicCameraController.queuedSceneOverride) return;

        __instance.loadScene = "ProtoCredits";
    }
}