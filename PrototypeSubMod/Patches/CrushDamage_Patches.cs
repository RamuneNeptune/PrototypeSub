using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CrushDamage))]
public class CrushDamage_Patches
{
    [HarmonyPatch(nameof(CrushDamage.UpdateDepthClassification)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateDepthClassification_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var match = new CodeMatch(i => i.opcode == OpCodes.Callvirt);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(DontAllowDepthUpdate));
        
        return matcher.InstructionEnumeration();
    }

    public static bool DontAllowDepthUpdate(bool prevValue, CrushDamage crushDamage)
    {
        if (!crushDamage.transform.parent) return prevValue;

        return crushDamage.transform.parent.name != "ProtoVehicleHolder";
    }
}