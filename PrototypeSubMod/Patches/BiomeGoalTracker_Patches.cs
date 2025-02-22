using HarmonyLib;
using Story;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(BiomeGoalTracker))]
internal class BiomeGoalTracker_Patches
{
    private static bool TrackingBlocked;

    [HarmonyPatch(nameof(BiomeGoalTracker.TrackBiome)), HarmonyPrefix]
    private static bool TrackBiome_Prefix()
    {
        return !TrackingBlocked;
    }

    public static void SetTrackingBlocked(bool blocked)
    {
        TrackingBlocked = blocked;
    }
}
