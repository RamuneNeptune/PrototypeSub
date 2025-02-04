using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(IngameMenu))]
internal class IngameMenu_Patches
{
    private static bool _allowSaving = true;

    [HarmonyPatch(nameof(IngameMenu.GetAllowSaving)), HarmonyPostfix]
    private static void GetAllowSaving_Postfix(ref bool __result)
    {
        if (!_allowSaving) return;

        __result = false;
    }

    public static void SetAllowSavingOverride(bool allowSaving)
    {
        _allowSaving = allowSaving;
    }
}
