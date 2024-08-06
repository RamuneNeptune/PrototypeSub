using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(PrecursorTeleporter))]
internal class PrecursorTeleporterPatches
{
    [HarmonyPatch(nameof(PrecursorTeleporter.Start)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Start_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var exitPointField = typeof(PrecursorTeleporter).GetField("registerPrisonExitPoint", BindingFlags.Public | BindingFlags.Instance);
        var fieldMatch = new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == exitPointField);

        var labelMatcher = new CodeMatcher(instructions)
            .MatchForward(false, fieldMatch)
            .Advance(3);

        var label = labelMatcher.Instruction.operand;
        var matcher = new CodeMatcher(instructions)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(HasValuesInitialized))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Brfalse_S, label));

        return matcher.InstructionEnumeration();
    }

    public static bool HasValuesInitialized(PrecursorTeleporter instance)
    {
        if (instance.portalFxPrefab == null) return false;

        return true;
    }
}
