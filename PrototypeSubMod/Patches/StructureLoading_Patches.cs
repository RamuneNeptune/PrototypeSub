using HarmonyLib;
using ModStructureFormat;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

internal class StructureLoading_Patches
{
    public static IEnumerable<CodeInstruction> RegisterStructure_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        var labelMatch = new CodeMatch(i => i.opcode == OpCodes.Stind_I4);
        var arrayMatch = new CodeMatch(i => i.opcode == OpCodes.Stfld);

        var matcher = new CodeMatcher(instructions, generator)
            .MatchForward(false, labelMatch)
            .Advance(1)
            .CreateLabel(out Label label)
            .MatchBack(false, arrayMatch);

        var entity = matcher.Instruction.operand;

        matcher.Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_2))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, entity))
            .InsertAndAdvance(Transpilers.EmitDelegate(EntityIsBlacklistedGlassray))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brtrue, label));

        return matcher.InstructionEnumeration();
    }

    public static bool EntityIsBlacklistedGlassray(Entity entity)
    {
        return entity.id == "b82f582e-3894-4aaa-882f-506e191f591a";
    }
}
