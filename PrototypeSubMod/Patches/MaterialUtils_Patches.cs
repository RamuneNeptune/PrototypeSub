using HarmonyLib;
using Nautilus.Utility;
using Nautilus.Utility.MaterialModifiers;
using PrototypeSubMod.Utility;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MaterialUtils))]
internal class MaterialUtils_Patches
{
    [SaveStateReference(false)]
    private static bool calledFromAssembly;
    
    [HarmonyPatch(nameof(MaterialUtils.ApplyUBERShader)), HarmonyPrefix]
    private static bool ApplyUBERShader_Prefix(Material material, ref (bool, float) __state)
    {
        bool isSNShader = material.shader == MaterialUtils.Shaders.MarmosetUBER ||
                          material.shader == MaterialUtils.Shaders.ParticlesUBER ||
                          material.shader == MaterialUtils.Shaders.IonCube;
        
        __state.Item1 = material.HasProperty("_GlossMapScale");
        if (!__state.Item1) return !isSNShader;

        __state.Item2 = material.GetFloat("_GlossMapScale");

        return !isSNShader;
    }

    [HarmonyPatch(nameof(MaterialUtils.ApplyUBERShader)), HarmonyPostfix]
    private static void ApplyUBERShader_Postfix(Material material, (bool, float) __state)
    {
        if (!__state.Item1 || !calledFromAssembly) return;

        material.SetFloat("_Shininess", __state.Item2 * 8f);
        calledFromAssembly = false;
    }

    [HarmonyPatch(nameof(MaterialUtils.ApplySNShaders)), HarmonyPrefix]
    private static void ApplySNShaders_Prefix()
    {
        calledFromAssembly = Assembly.GetCallingAssembly() == Plugin.Assembly;
    }

    [HarmonyPatch(nameof(MaterialUtils.ApplySNShaders)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ApplySNShaders_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var methodParams = new[] { typeof(Material), typeof(float), typeof(float), typeof(float), typeof(MaterialUtils.MaterialType) };
        var method = AccessTools.Method(typeof(MaterialUtils), nameof(MaterialUtils.ApplyUBERShader), methodParams);

        var match = new CodeMatch(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == method);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(-7);

        var blockShaderConv = matcher.Instruction.operand;

        matcher
            .Advance(-26);

        var modifiers = matcher.Instruction.operand;

        matcher
            .Advance(26)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldelem_Ref))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_S, modifiers))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, blockShaderConv))
            .Insert(Transpilers.EmitDelegate(CallPreConversionMethod));

        return matcher.InstructionEnumeration();
    }

    public static void CallPreConversionMethod(Renderer rend, Material mat, MaterialModifier[] modifiers, bool blockShaderConversion)
    {
        if (blockShaderConversion) return;

        foreach (var modifier in modifiers)
        {
            if (modifier is not ProtoMaterialModifier) continue;

            (modifier as ProtoMaterialModifier).OnPreShaderConversion(mat, rend);
        }
    }
}
