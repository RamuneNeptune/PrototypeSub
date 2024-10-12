using HarmonyLib;
using Nautilus.Utility;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(MaterialUtils))]
internal class MaterialUtilsPatches
{
    [HarmonyPatch(nameof(MaterialUtils.ApplyUBERShader)), HarmonyPrefix]
    private static void ApplyUBERShader_Prefix(Material material, ref float __state)
    {
        __state = material.GetFloat("_GlossMapScale");
    }

    [HarmonyPatch(nameof(MaterialUtils.ApplyUBERShader)), HarmonyPostfix]
    private static void ApplyUBERShader_Postfix(Material material, float __state)
    {
        material.SetFloat("_Smoothness", __state * 8f);
    }
}
