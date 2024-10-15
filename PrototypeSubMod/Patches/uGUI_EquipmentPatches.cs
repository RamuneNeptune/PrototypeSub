using HarmonyLib;
using PrototypeSubMod.DeployablesTerminal;
using PrototypeSubMod.PowerSystem;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_Equipment))]
internal class uGUI_EquipmentPatches
{
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

        go.transform.localPosition = new Vector3(142, -110, 0);
        go.transform.localScale = new Vector3(6, 6, 1);
        go.AddComponent<CanvasRenderer>();
        var img = go.AddComponent<Image>();
        img.sprite = Plugin.AssetBundle.LoadAsset<Sprite>("DecoyStorageBackground");
        img.raycastTarget = false;

        var powerAbilitySlot = CloneSlots(__instance, new[] { ProtoPowerAbilitySystem.SlotName }, "DecoySlot", null);
        GameObject consumeButton = new GameObject();
        consumeButton.transform.SetParent(__instance.transform);
        consumeButton.name = "PowerAbilityConsumeButton";

        var rect = consumeButton.AddComponent<RectTransform>();
        rect.localPosition = new Vector3(0, -250, 0);
        float size = 0.05f;
        rect.sizeDelta = new Vector2(1.78f * size, size);

        consumeButton.AddComponent<Image>().sprite = Plugin.AssetBundle.LoadAsset<Sprite>("Proto_ConsumeButton");
        var button = consumeButton.AddComponent<Button>();
        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = new SpriteState()
        {
            selectedSprite = Plugin.AssetBundle.LoadAsset<Sprite>("Proto_ConsumeButton"),
            highlightedSprite = Plugin.AssetBundle.LoadAsset<Sprite>("Proto_ConsumeButton_Hovered"),
            pressedSprite = Plugin.AssetBundle.LoadAsset<Sprite>("Proto_ConsumeButton_Clicked"),
            disabledSprite = Plugin.AssetBundle.LoadAsset<Sprite>("Proto_ConsumeButton_Disabled")
        };

        button.onClick.AddListener(ProtoPowerAbilitySystem.Instance.ConsumeItem);

        consumeButton.SetActive(false);
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
            var clonedSlot = CloneSlot(equipment, $"{copyTarget}{i + 1}", slots[i]);
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

    [HarmonyPatch(nameof(uGUI_Equipment.Init)), HarmonyPostfix]
    private static void Init_Postfix(uGUI_Equipment __instance, Equipment equipment)
    {
        bool isAbilitySystem = equipment._label == ProtoPowerAbilitySystem.EquipmentLabel;
        __instance.transform.Find("PowerAbilityConsumeButton").gameObject.SetActive(isAbilitySystem);
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
