using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsDamagePoint))]
internal class DamagePointPatches
{
    [HarmonyPatch(nameof(CyclopsDamagePoint.SpawnFx)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SpawnFX_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new CodeMatch(i => i.opcode == OpCodes.Stfld);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(TryGetParticleInChildren));

        return matcher.InstructionEnumeration();
    }

    public static ParticleSystem TryGetParticleInChildren(ParticleSystem particleSystem, GameObject prefab)
    {
        if (particleSystem != null) return particleSystem;

        var newParticles = prefab.GetComponentInChildren<ParticleSystem>();
        return newParticles;
    }

    [HarmonyPatch(nameof(CyclopsDamagePoint.OnRepair)), HarmonyPrefix]
    private static void OnRepair_Prefix(CyclopsDamagePoint __instance)
    {
        if (__instance.ps == null) return;

        var damagePoint = __instance.ps.GetComponentInParent<CyclopsDamagePoint>();
        if (damagePoint == __instance)
        {
            GameObject.Destroy(__instance.gameObject);
        }
    }
}
