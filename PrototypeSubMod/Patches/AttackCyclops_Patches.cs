using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(AttackCyclops))]
internal class AttackCyclops_Patches
{
    [HarmonyPatch(nameof(AttackCyclops.OnCollisionEnter)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnCollisionEnter_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var field = typeof(AttackCyclops).GetField("agressiveToNoise", BindingFlags.Instance | BindingFlags.Public);
        var match = new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == field);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(-6)
            .InsertAndAdvance(Transpilers.EmitDelegate(RedirectCurrentTarget));

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(AttackCyclops.SetCurrentTarget)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SetCurrentTarget_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .InsertAndAdvance(Transpilers.EmitDelegate(RedirectCurrentTarget))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Starg, 1));

        return matcher.InstructionEnumeration();
    }

    public static GameObject RedirectCurrentTarget(GameObject target)
    {
        if (target == null) return target;

        var handler = target.GetComponentInChildren<CloakEffectHandler>();
        if (!handler) return target;

        if (!handler.GetAllowedToCloak()) return target;

        Player_Patches.DummyLDFTarget.transform.position = handler.GetContinuousPointOnSurface(1.1f);
        return Player_Patches.DummyLDFTarget;
    }
}
