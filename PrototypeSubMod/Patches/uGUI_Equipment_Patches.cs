using HarmonyLib;
using PrototypeSubMod.DeployablesTerminal;
using PrototypeSubMod.PowerSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using PrototypeSubMod.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_Equipment))]
internal class uGUI_Equipment_Patches
{
    [SaveStateReference]
    private static DraggedItem LastDraggedItem;

    [HarmonyPatch(nameof(uGUI_Equipment.Awake)), HarmonyPrefix]
    private static void Awake_Prefix(uGUI_Equipment __instance)
    {
        CloneSlots(__instance, PrototypePowerSystem.SLOT_NAMES);

        var slot0 = CloneSlots(__instance, DeployablesStorageTerminal.SLOT_NAMES, "BatteryCharger", null, DeployablesStorageTerminal.SLOT_POSITIONS);

        GameObject go = new();
        go.transform.SetParent(slot0.transform);
        go.name = "DecoyStorageBackground";

        go.AddComponent<RectTransform>();

        go.transform.localPosition = new Vector3(152, 57, 0);
        go.transform.localScale = new Vector3(8, 10.3f, 1);
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.sprite = Plugin.AssetBundle.LoadAsset<Sprite>("Proto_DeployablesBG");
        img.raycastTarget = false;

        var powerAbilitySlot = CloneSlots(__instance, new[] { ProtoPowerAbilitySystem.SlotName }, "DecoySlot", null);
        GameObject consumeButton = GameObject.Instantiate(Plugin.AssetBundle.LoadAsset<GameObject>("PowerAbilityConsumeButton"), __instance.transform);
        consumeButton.transform.localPosition = new Vector3(0, -245, 0);
        consumeButton.SetActive(false);

        var background = new GameObject();
        background.name = "AbilityButtonBackground";
        background.transform.SetParent(powerAbilitySlot.transform);
        var rect = background.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(800, 1030);
        var image = background.AddComponent<Image>();
        image.sprite = Plugin.AssetBundle.LoadAsset<Sprite>("PowerDepotBackground");
        image.raycastTarget = false;
        background.transform.localPosition = new Vector3(0, -200, 0);
        background.transform.localScale = Vector3.one;
        background.transform.localRotation = Quaternion.identity;
    }

#nullable enable
    private static uGUI_EquipmentSlot? CloneSlots(uGUI_Equipment equipment, string[] slots, string copyTarget = "SeamothModule", string? imageTarget = "Seamoth", Vector3[]? slotPositions = null)
    {
        if (slots.Length == 0) return null;

        uGUI_EquipmentSlot slot = CloneSlot(equipment, $"{copyTarget}1", slots[0]);
        if (imageTarget != null)
        {
            GameObject.Destroy(slot.transform.Find(imageTarget).GetComponent<Image>());
        }

        if (slotPositions != null)
        {
            slot.transform.localPosition = slotPositions[0];
        }

        for (int i = 1; i < slots.Length; i++)
        {
            var clonedSlot = CloneSlot(equipment, $"{copyTarget}{Mathf.Min(4, i + 1)}", slots[i]);
            if (slotPositions != null)
            {
                clonedSlot.transform.localPosition = slotPositions[i];
            }
        }

        return slot;
    }
#nullable disable

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
            .Insert(Transpilers.EmitDelegate(Inventory_Patches.GetModifiedEquipmentTypeItemsContainer));

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(uGUI_Equipment.CanSwitchOrSwap)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> CanSwitchOrSwap_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatch match = new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "GetEquipmentType");

        FieldInfo containerInfo = typeof(InventoryItem).GetField("container");

        var matcher = new CodeMatcher(instructions)
            .MatchForward(true, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, containerInfo))
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
            .Insert(Transpilers.EmitDelegate(Inventory_Patches.GetModifiedEquipmentTypeItemsContainer));

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

    [HarmonyPatch(nameof(uGUI_Equipment.Init)), HarmonyPostfix]
    private static void Init_Postfix(uGUI_Equipment __instance, Equipment equipment)
    {
        bool isAbilitySystem = equipment._label == ProtoPowerAbilitySystem.EquipmentLabel;
        __instance.GetComponentInChildren<AbilityConsumptionButton>(true).gameObject.SetActive(isAbilitySystem);
    }

    private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName)
    {
        Transform newSlot = GameObject.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
        newSlot.name = newSlotName;
        uGUI_EquipmentSlot equipmentSlot = newSlot.GetComponent<uGUI_EquipmentSlot>();
        equipmentSlot.slot = newSlotName;
        return equipmentSlot;
    }

    [HarmonyPatch(nameof(uGUI_Equipment.SelectItemInDirection)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SelectItemInDirection_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var clear = typeof(List<ISelectable>).GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
        var match = new CodeMatch(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == clear);

        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, match)
            .Advance(1)
            .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
            .InsertAndAdvance(Transpilers.EmitDelegate(EnsureAbilityButtonAdded));

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(uGUI_Equipment.selectedSlot)), HarmonyPatch(MethodType.Getter), HarmonyPostfix]
    private static void SelectedSlot_Postfix(uGUI_Equipment __instance, ref uGUI_EquipmentSlot __result)
    {
        if (UISelection.selected is AbilityConsumptionButton button)
        {
            __result = button.GetDummySlot();
        }
    }

    [HarmonyPatch(nameof(uGUI_Equipment.SelectItem)), HarmonyPrefix]
    private static bool SelectItem_Prefix(uGUI_Equipment __instance, object item)
    {
        if (item is AbilityConsumptionButton button)
        {
            __instance.DeselectItem();
            UISelection.selected = button;
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(uGUI_Equipment.GetSelectedItem)), HarmonyPrefix]
    private static bool GetSelectedItem_Prefix(uGUI_Equipment __instance, ref object __result)
    {
        if (UISelection.selected is AbilityConsumptionButton button)
        {
            __result = button;
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(uGUI_Equipment.GetSelectedIcon)), HarmonyPrefix]
    private static bool GetSelectedIcon_Prefix(uGUI_Equipment __instance, ref Graphic __result)
    {
        if (UISelection.selected is AbilityConsumptionButton button)
        {
            __result = button.GetGraphic();
            return false;
        }

        return true;
    }

    public static void EnsureAbilityButtonAdded(uGUI_Equipment instance)
    {
        var abilityButton = instance.GetComponentInChildren<AbilityConsumptionButton>();
        if (!abilityButton) return;

        UISelection.sSelectables.Add(abilityButton);
    }

    public static bool OverwriteNullSelection(bool previousWasNull)
    {
        if (UISelection.selected is AbilityConsumptionButton) return true;

        return previousWasNull;
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
