using HarmonyLib;
using PrototypeSubMod.Monobehaviors;
using UnityEngine;

namespace PrototypeSubMod.Patches;

[HarmonyPatch(typeof(uGUI_Equipment))]
internal class uGUI_EquipmentPatches
{
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

    private static uGUI_EquipmentSlot CloneSlot(uGUI_Equipment equipmentMenu, string childName, string newSlotName)
    {
        Transform newSlot = GameObject.Instantiate(equipmentMenu.transform.Find(childName), equipmentMenu.transform);
        newSlot.name = newSlotName;
        uGUI_EquipmentSlot equipmentSlot = newSlot.GetComponent<uGUI_EquipmentSlot>();
        equipmentSlot.slot = newSlotName;
        return equipmentSlot;
    }
}
