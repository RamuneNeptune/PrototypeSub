using System;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class ProtoPowerAbilitySystem : MonoBehaviour
{
    public static readonly string EquipmentLabel = "ProtoPowerExtractorLabel";
    public static readonly string SlotName = "ProtoPowerConsumptionSlot";

    public static ProtoPowerAbilitySystem Instance { get; private set; }

    public Equipment equipment { get; private set; }
    public Action onEquip;
    public Action onUnequip;

    [SerializeField] private ChildObjectIdentifier storageRoot;
    [SerializeField] private ChildObjectIdentifier functionalityRoot;
    [SerializeField] private FMODAsset equipSound;
    [SerializeField] private FMODAsset unequipSound;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            throw new System.Exception($"More than 1 ProtoPowerAbilitySystem in the scene! Destroying {this}");
        }

        Instance = this;

        Initialize();
    }

    private void Initialize()
    {
        if (equipment != null) return;

        equipment = new(gameObject, storageRoot.transform);
        equipment.SetLabel(EquipmentLabel);
        equipment.onEquip += OnEquip;
        equipment.onUnequip += OnUnequip;

        equipment.AddSlots(new[] { SlotName });

        equipment.isAllowedToAdd = IsAllowedToAdd;
        equipment.isAllowedToRemove = (p, v) =>
        {
            return true;
        };
    }

    public void OnHover(HandTargetEventData eventData)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UsePowerAbilitySystem", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(equipment);
        pda.Open(PDATab.Inventory);
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        FMODUWE.PlayOneShot(equipSound, transform.position, 1f);
        onEquip?.Invoke();
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        FMODUWE.PlayOneShot(unequipSound, transform.position, 1f);
        onUnequip?.Invoke();
    }

    private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
    {
        return PrototypePowerSystem.AllowedPowerSources.Keys.Contains(pickupable.GetTechType());
    }

    public void ConsumeItem()
    {
        var currentItem = equipment.GetItemInSlot(SlotName);
        var effectType = PrototypePowerSystem.AllowedPowerSources[currentItem.techType].sourceEffectFunctionality;
        if (effectType != null)
        {
            functionalityRoot.gameObject.AddComponent(effectType);
        }

        equipment.RemoveItem(SlotName, true, true);
        Destroy(currentItem.item.gameObject);
    }

    public bool HasItem()
    {
        return equipment.GetItemInSlot(SlotName) != null;
    }
}
