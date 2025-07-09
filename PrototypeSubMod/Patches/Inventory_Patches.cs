﻿using HarmonyLib;
using PrototypeSubMod.PowerSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(Inventory))]
internal class Inventory_Patches
{
    [HarmonyPatch(nameof(Inventory.AddOrSwap)), HarmonyTranspiler]
    [HarmonyPatch(new Type[] { typeof(InventoryItem), typeof(Equipment), typeof(string) })]
    private static IEnumerable<CodeInstruction> AddOrSwap_Equipment_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .Insert(Transpilers.EmitDelegate(GetModifiedEquipmentType));

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(Inventory.GetAllItemActions)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GetAllItemActions_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        MethodInfo methodInfo = typeof(Inventory).GetMethod("GetOppositeContainer");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)) //Load the Inventory instance (this)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1)) //Load the InventoryItem
            .InsertAndAdvance(new CodeInstruction(OpCodes.Call, methodInfo)) //Get the container on the right side of the inventory
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1)) //Load the InventoryItem
            .Insert(Transpilers.EmitDelegate(GetModifiedEquipmentTypeItemsContainer));

        return matcher.InstructionEnumeration();
    }

    public static EquipmentType GetModifiedEquipmentType(EquipmentType originalType, InventoryItem itemA, Equipment equipmentB)
    {
        if (itemA == null) return originalType;

        if (!equipmentB.owner) return originalType;

        bool isPowerEquipment = equipmentB.typeToSlots.ElementAt(0).Key == Plugin.DummyPowerType;
        if (!isPowerEquipment) return originalType;

        if (PrototypePowerSystem.AllowedPowerSources.Keys.Contains(itemA.techType))
        {
            return Plugin.PrototypePowerType;
        }

        return originalType;
    }

    public static EquipmentType GetModifiedEquipmentTypeItemsContainer(EquipmentType originalType, IItemsContainer container, InventoryItem itemA)
    {
        if (itemA == null) return originalType;

        bool transferContainer = container.label != PrototypePowerSystem.EquipmentLabel && container.label != ProtoPowerAbilitySystem.EquipmentLabel;

        if (transferContainer)
        {
            return originalType;
        }

        if (PrototypePowerSystem.AllowedPowerSources.Keys.Contains(itemA.techType))
        {
            return Plugin.PrototypePowerType;
        }

        return originalType;
    }
}
