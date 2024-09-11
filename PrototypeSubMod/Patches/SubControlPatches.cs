using HarmonyLib;
using PrototypeSubMod.MotorHandler;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(SubControl))]
internal class SubControlPatches
{
    [HarmonyPatch(nameof(SubControl.FixedUpdate)), HarmonyPrefix]
    private static bool FixedUpdate_Prefix(SubControl __instance)
    {
        var motorHandler = __instance.GetComponentInChildren<ProtoMotorHandler>();
        if (!motorHandler) return true;

        return motorHandler.GetAllowedToMove();
    }

    [HarmonyPatch(nameof(SubControl.UpdateAnimation)), HarmonyPrefix]
    private static bool UpdateAnimation_Prefix(SubControl __instance)
    {
        var motorHandler = __instance.GetComponentInChildren<ProtoMotorHandler>();
        if (!motorHandler) return true;

        if(!motorHandler.GetAllowedToMove())
        {
            __instance.mainAnimator.SetFloat("view_yaw", 0);
            __instance.mainAnimator.SetFloat("view_pitch", 0);
            Player.main.playerAnimator.SetFloat("cyclops_yaw", 0);
            Player.main.playerAnimator.SetFloat("cyclops_pitch", 0);
            return false;
        }

        return true;
    }
}