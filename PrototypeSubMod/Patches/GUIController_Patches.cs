using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(GUIController))]
internal class GUIController_Patches
{
    private static bool denyCycling;

    [HarmonyPatch(nameof(GUIController.Update)), HarmonyPrefix]
    private static bool Update_Prefix()
    {
        return !denyCycling;
    }

    public static void SetDenyHideCycling(bool deny)
    {
        denyCycling = deny;
    }
}
