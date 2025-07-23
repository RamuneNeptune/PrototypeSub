using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(CraftTree))]
public class CraftTree_Patches
{
    [HarmonyPatch(nameof(CraftTree.Initialize)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Initialize_Postfix(IEnumerable<CodeInstruction> instructions)
    {
        var match = new CodeMatch[]
        {
            new(i => i.opcode == OpCodes.Ldstr && (string)i.operand == "Fabricator"),
            new(i => i.opcode == OpCodes.Call),
            new(i => i.opcode == OpCodes.Newobj),
            new(i => i.opcode == OpCodes.Stsfld)
        };

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(RemovePrecursorKeys));

        return matcher.InstructionEnumeration();
    }

    private static void RemovePrecursorKeys()
    {
        var personalTab = CraftTree.fabricator.nodes.nodes.FirstOrDefault(n => n.id == "Personal");
        if (personalTab == null) return;

        var equipmentTab = personalTab.nodes.FirstOrDefault(n => n.id == "Equipment");
        if (equipmentTab == null) return;

        equipmentTab.nodes.RemoveAll(n => n.id.StartsWith("PrecursorKey_"));
    }
}