using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsNoiseManager))]
internal class CyclopsNoiseManager_Patches
{
    [HarmonyPatch(nameof(CyclopsNoiseManager.RecalculateNoiseValues)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RecalculateNoiseValues_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo clamp01Info = typeof(Mathf).GetMethod("Clamp01", BindingFlags.Static | BindingFlags.Public);

        var match = new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == clamp01Info);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(-2)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(GetCloakMultiplier));

        return matcher.InstructionEnumeration();
    }

    public static float GetCloakMultiplier(float previousValue, CyclopsNoiseManager instance)
    {
        var effectHandler = instance.subRoot.gameObject.GetComponentInChildren<CloakEffectHandler>();
        if (!effectHandler) return previousValue;

        if (effectHandler.GetUpgradeInstalled() && effectHandler.GetUpgradeEnabled())
        {
            return effectHandler.soundMultiplier;
        }

        return previousValue;
    }
}
