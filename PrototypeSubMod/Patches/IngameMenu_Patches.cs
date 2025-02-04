using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(IngameMenu))]
internal class IngameMenu_Patches
{
    private static bool _denySaving = false;

    [HarmonyPatch(nameof(IngameMenu.GetAllowSaving)), HarmonyPostfix]
    private static void GetAllowSaving_Postfix(ref bool __result)
    {
        if (!_denySaving) return;

        __result = false;
    }

    public static void SetDenySaving(bool denySaving)
    {
        _denySaving = denySaving;
    }
}
