using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CyclopsDamagePoint))]
internal class DamagePoint_Patches
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
}
