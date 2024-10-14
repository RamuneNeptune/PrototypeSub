using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MaterialUtils))]
internal class MaterialUtilsPatches
{
    [HarmonyPatch(nameof(MaterialUtils.ApplyUBERShader)), HarmonyPrefix]
    private static void ApplyUBERShader_Prefix(Material material, ref (bool, float) __state)
    {
        __state.Item1 = material.HasProperty("_GlossMapScale");
        if (!__state.Item1) return;

        __state.Item2 = material.GetFloat("_GlossMapScale");
    }

    [HarmonyPatch(nameof(MaterialUtils.ApplyUBERShader)), HarmonyPostfix]
    private static void ApplyUBERShader_Postfix(Material material, (bool, float) __state)
    {
        if (!__state.Item1) return;

        material.SetFloat("_Shininess", __state.Item2 * 8f);
    }
}
