using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(TooltipFactory))]
internal class TooltipFactoryPatches
{
    // Power source efficiency tooltips

    /*
    [HarmonyPatch(nameof(TooltipFactory.ItemCommons)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ItemCommons_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch[] matches = new[]
        {
            new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "WriteDescription"),
            new CodeMatch(i => i.opcode == OpCodes.Ldc_I4_0)
        };

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, matches)
            .Advance(-1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_2))
            .Insert(new CodeInstruction(Transpilers.EmitDelegate(WriteTooltipDescription)));

        return matcher.InstructionEnumeration();
    }

    public static void WriteTooltipDescription(StringBuilder sb, TechType techType, GameObject obj)
    {
        if (!obj.TryGetComponent(out PrototypePowerBattery battery))
        {
            return;
        }
        string tooltip = Language.main.Get($"{techType}_ProtoEfficiency");

        TooltipFactory.WriteDescription(sb, tooltip);
    }
    */
}
