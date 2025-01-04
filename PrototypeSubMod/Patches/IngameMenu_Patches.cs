using HarmonyLib;
using PrototypeSubMod.PrototypeStory;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(IngameMenu))]
internal class IngameMenu_Patches
{
    [HarmonyPatch(nameof(IngameMenu.GetAllowSaving)), HarmonyPostfix]
    private static void GetAllowSaving_Postfix(ref bool __result)
    {
        if (!ProtoStoryLocker.WithinSaveLockZone) return;

        __result = false;
    }
}
