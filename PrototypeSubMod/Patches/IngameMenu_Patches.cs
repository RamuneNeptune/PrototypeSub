using HarmonyLib;
using PrototypeSubMod.PrototypeStory;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(IngameMenu))]
internal class IngameMenu_Patches
{
    private static void GetAllowSaving_Postfix(ref bool __result)
    {
        if (!ProtoStoryLocker.StoryEndingActive) return;

        __result = true;
    }
}
