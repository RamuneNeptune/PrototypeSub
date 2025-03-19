using HarmonyLib;
using PrototypeSubMod.Utility;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(GUIController))]
internal class GUIController_Patches
{
    [SaveStateReference(false)]
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
