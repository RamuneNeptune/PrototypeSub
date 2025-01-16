using HarmonyLib;
using PrototypeSubMod.PrototypeStory;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(LiveMixin))]
internal class LiveMixin_Patches
{
    [HarmonyPatch(nameof(LiveMixin.TakeDamage)), HarmonyPrefix]
    private static bool TakeDamage_Prefix(LiveMixin __instance)
    {
        if (Player.main.liveMixin != __instance)
        {
            return true;
        }

        return !ProtoStoryLocker.WithinSaveLockZone;
    }
}
