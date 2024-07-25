using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_ItemsContainer))]
internal class uGUI_ItemsContainerPatches
{
    [HarmonyPatch(nameof(uGUI_ItemsContainer.SelectItem)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SelectItem_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        FieldInfo containerInfo = typeof(InventoryItem).GetField("container");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, containerInfo))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_1))
            .Insert(Transpilers.EmitDelegate(InventoryPatches.GetModifiedEquipmentTypeItemsContainer));

        return matcher.InstructionEnumeration();
    }
}
