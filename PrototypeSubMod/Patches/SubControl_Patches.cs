using HarmonyLib;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.PrototypeStory;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(SubControl))]
internal class SubControl_Patches
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

        if (!motorHandler.GetAllowedToMove())
        {
            __instance.mainAnimator.SetFloat("view_yaw", 0);
            __instance.mainAnimator.SetFloat("view_pitch", 0);
            Player.main.playerAnimator.SetFloat("cyclops_yaw", 0);
            Player.main.playerAnimator.SetFloat("cyclops_pitch", 0);
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(SubControl.Update)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Update_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var throttleField = typeof(SubControl).GetField("throttle", BindingFlags.NonPublic | BindingFlags.Instance);
        var match = new CodeMatch(i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == throttleField);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .InsertAndAdvance(Transpilers.EmitDelegate(OverrideMoveDirIfNeeded));

        return matcher.InstructionEnumeration();
    }

    public static Vector3 OverrideMoveDirIfNeeded(Vector3 oldDir)
    {
        if (!ProtoStoryLocker.StoryEndingActive) return oldDir;

        return new Vector3(0, 0, 1);
    }
}