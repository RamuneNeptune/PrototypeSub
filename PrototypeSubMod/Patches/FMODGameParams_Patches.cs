using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(FMODGameParams))]
public class FMODGameParams_Patches
{
    [HarmonyPatch(nameof(FMODGameParams.UpdateParams)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateParams_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var getBiomeStringMethod = typeof(Player).GetMethod("GetBiomeString", BindingFlags.Public | BindingFlags.Instance);
        var biomeStringMatch =
            new CodeMatch(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == getBiomeStringMethod);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, biomeStringMatch)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(OverrideBiomeIfNeeded));

        return matcher.InstructionEnumeration();
    }

    public static string OverrideBiomeIfNeeded(string biomeName, FMODGameParams gameParams)
    {
        if (Player_Patches.BLACKLISTED_WALKABLE_BIOMES.Contains(biomeName) && gameParams.onlyInBiome == "Precursor")
        {
            return "Precursor_" + biomeName;
        }

        return biomeName;
    }
}