using HarmonyLib;
using PrototypeSubMod.MiscMonobehaviors;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.PrototypeStory;
using System;
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
        var throttleMatch = new CodeMatch(i => i.opcode == OpCodes.Stfld && (FieldInfo)i.operand == throttleField);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, throttleMatch)
            .InsertAndAdvance(Transpilers.EmitDelegate(OverrideMoveDirIfNeeded))
            .MatchForward(true, GetButtonMatch())
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(GetAllowedToToggleLights));

        return matcher.InstructionEnumeration();
    }

    public static Vector3 OverrideMoveDirIfNeeded(Vector3 oldDir)
    {
        if (!ProtoStoryLocker.StoryEndingActive) return oldDir;

        return new Vector3(0, 0, 1);
    }

    public static bool GetAllowedToToggleLights(bool wasClicking, SubControl subControl)
    {
        var blocker = subControl.GetComponentInChildren<BlockTogglingLights>();

        if (!blocker) return wasClicking;

        return false;
    }

    private static CodeMatch[] GetButtonMatch()
    {
        CodeMatch[] match = null;

        if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.aci.hydra") || BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.aci.callofthevoid"))
        {
            match = new[]
            {
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_5),
                new CodeMatch(i => i.opcode == OpCodes.Ldarg_0),
                new CodeMatch(i => i.opcode == OpCodes.Call)
            };
        }
        else
        {
            var getButtonDownMethod = typeof(GameInput).GetMethod("GetButtonDown", BindingFlags.Public | BindingFlags.Static);
            match = new[]
            {
                new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_5),
                new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == getButtonDownMethod)
            };
        }

        return match;
    }
}