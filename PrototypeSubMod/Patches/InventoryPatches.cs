using HarmonyLib;
using PrototypeSubMod.Monobehaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Inventory))]
internal class InventoryPatches
{
    [HarmonyPatch(nameof(Inventory.AddOrSwap)), HarmonyTranspiler]
    [HarmonyPatch(new Type[] { typeof(InventoryItem), typeof(Equipment), typeof(string) })]
    private static IEnumerable<CodeInstruction> AddOrSwap_Equipment_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .Insert(Transpilers.EmitDelegate(GetModifiedEquipmentType));

        return matcher.InstructionEnumeration();
    }

    public static EquipmentType GetModifiedEquipmentType(InventoryItem itemA, Equipment equipmentB, EquipmentType originalType)
    {
        if (equipmentB.tr.parent == null) return originalType;

        if (!equipmentB.tr.parent.TryGetComponent(out PrototypePowerSystem powerSystem)) return originalType;

        if(powerSystem.GetAllowedTechTypes().Contains(itemA.techType))
        {
            return Plugin.PrototypePowerType;
        }

        return originalType;
    }
}
