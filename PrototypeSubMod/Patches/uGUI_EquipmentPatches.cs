using HarmonyLib;
using PrototypeSubMod.Monobehaviors;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_Equipment))]
internal class uGUI_EquipmentPatches
{
    private static DraggedItem LastDraggedItem;

    [HarmonyPatch(nameof(uGUI_Equipment.Awake)), HarmonyPrefix]
    private static void Awake_Prefix(uGUI_Equipment __instance)
    {
        if (PrototypePowerSystem.SLOT_NAMES.Length == 0) return;

        uGUI_EquipmentSlot slot = CloneSlot(__instance, "SeamothModule1", PrototypePowerSystem.SLOT_NAMES[0]);

        for (int i = 1; i < PrototypePowerSystem.SLOT_NAMES.Length; i++)
        {
            CloneSlot(__instance, $"SeamothModule{i + 1}", PrototypePowerSystem.SLOT_NAMES[i]);
        }
    }

    [HarmonyPatch(nameof(uGUI_Equipment.OnItemDragStart)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnItemDragStart_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        FieldInfo inventoryItemInfo = typeof(Pickupable).GetField("inventoryItem", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo containerInfo = typeof(InventoryItem).GetField("container");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, inventoryItemInfo))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, containerInfo))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_1))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, inventoryItemInfo))
            .Insert(Transpilers.EmitDelegate(InventoryPatches.GetModifiedEquipmentTypeItemsContainer));

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(uGUI_Equipment.CanSwitchOrSwap)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CanSwitchOrSwap_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        //FieldInfo inventoryItemInfo = typeof(Pickupable).GetField("inventoryItem", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo containerInfo = typeof(InventoryItem).GetField("container");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, containerInfo))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .Insert(Transpilers.EmitDelegate(InventoryPatches.GetModifiedEquipmentTypeItemsContainer)); 

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(uGUI_Equipment.OnItemDragStart)), HarmonyPrefix]
    private static void OnItemDragStart_Prefix(Pickupable p)
    {
        TechType type = p.GetTechType();
        if (!PrototypePowerSystem.AllowedPowerSources.Keys.Contains(type)) return;
        LastDraggedItem = new DraggedItem(p, CraftData.GetEquipmentType(type));
        CraftData.equipmentTypes[type] = Plugin.PrototypePowerType;
    }

    [HarmonyPatch(nameof(uGUI_Equipment.OnItemDragStop)), HarmonyPrefix]
    private static void OnItemDragStop_Prefix()
    {
        if (LastDraggedItem == null)
        {
            //Invalid slot
            return;
        }

        CraftData.equipmentTypes[LastDraggedItem.pickupable.GetTechType()] = LastDraggedItem.originalType;
    }

    private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName)
    {
        Transform newSlot = GameObject.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
        newSlot.name = newSlotName;
        uGUI_EquipmentSlot equipmentSlot = newSlot.GetComponent<uGUI_EquipmentSlot>();
        equipmentSlot.slot = newSlotName;
        return equipmentSlot;
    }

    private class DraggedItem
    {
        public Pickupable pickupable;
        public EquipmentType originalType;
        public DraggedItem(Pickupable pickupable, EquipmentType originalType)
        {
            this.pickupable = pickupable;
            this.originalType = originalType;
        }
    }
}
