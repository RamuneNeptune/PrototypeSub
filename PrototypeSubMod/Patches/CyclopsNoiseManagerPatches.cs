using HarmonyLib;
using PrototypeSubMod.LightDistortionField;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsNoiseManager))]
internal class CyclopsNoiseManagerPatches
{
    [HarmonyPatch(nameof(CyclopsNoiseManager.RecalculateNoiseValues)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> RecalculateNoiseValues_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo clamp01Info = typeof(Mathf).GetMethod("Clamp01", BindingFlags.Static | BindingFlags.Public);
        PropertyInfo noiseScalarInfo = typeof(CyclopsNoiseManager).GetProperty("noiseScalar", BindingFlags.Instance | BindingFlags.Public);
        FieldInfo subRootInfo = typeof(CyclopsNoiseManager).GetField("subRoot", BindingFlags.Instance | BindingFlags.Public);

        var match = new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == clamp01Info);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(-5)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, noiseScalarInfo.GetGetMethod(false)))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, subRootInfo))
            .InsertAndAdvance(Transpilers.EmitDelegate(GetCloakMultiplier))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Mul))
            .Insert(new CodeInstruction(OpCodes.Call, noiseScalarInfo.GetSetMethod(true)));

        foreach (var instruction in matcher.InstructionEnumeration())
        {
            Plugin.Logger.LogInfo($"{instruction.opcode} {instruction.operand}");
        }

        return matcher.InstructionEnumeration();
    }

    public static float GetCloakMultiplier(SubRoot subRoot)
    {
        var effectHandler = subRoot.gameObject.GetComponentInChildren<CloakEffectHandler>();
        if (!effectHandler) return 1f;

        if(effectHandler.effectEnabled)
        {
            return effectHandler.soundMultiplier;
        }

        return 1f;
    }
}
