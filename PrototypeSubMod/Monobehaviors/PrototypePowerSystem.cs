using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class PrototypePowerSystem : MonoBehaviour
{
    public static readonly string[] SLOT_NAMES = new string[]
    {
        "PrototypePowerSlot1",
        "PrototypePowerSlot2",
        "PrototypePowerSlot3",
        "PrototypePowerSlot4"
    };

    public static readonly Dictionary<TechType, float> AllowedPowerSources = new()
    {
        { TechType.PowerCell, 1000 },
        { TechType.PrecursorIonCrystal, 1000 },
        { TechType.PrecursorIonPowerCell, 1000 },
    };

    public static readonly string EquipmentLabel = "PrototypePowerLabel";

    public Equipment equipment { get; private set; }

    [SerializeField] private Transform storageRoot;
    [SerializeField] private BatterySource[] batterySources;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if(batterySources.Length != SLOT_NAMES.Length)
        {
            Plugin.Logger.LogError($"Battery source and slot name length mismatch on {gameObject}!");
        }
    }

    private void Initialize()
    {
        if (equipment != null) return;

        equipment = new(gameObject, storageRoot);
        equipment.SetLabel(EquipmentLabel);
        equipment.onEquip += OnEquip;
        equipment.onUnequip += OnUnequip;

        equipment.AddSlots(SLOT_NAMES);

        equipment.isAllowedToAdd = IsAllowedToAdd;
        equipment.isAllowedToRemove = (p, v) =>
        {
            return true;
        };
    }

    private void OnEquip(string slot, InventoryItem item)
    {
        Plugin.Logger.LogInfo($"Equipped {item?.techType} to slot {slot} on {gameObject}");

        int index = Array.IndexOf(SLOT_NAMES, slot);

        var batterySource = batterySources[index];
        float power = AllowedPowerSources[item.techType];
    }
    
    private void OnUnequip(string slot, InventoryItem item)
    {
        Plugin.Logger.LogInfo($"Unequipped {item?.techType} from slot {slot} on {gameObject}");

        int index = Array.IndexOf(SLOT_NAMES, slot);

        var batterySource = batterySources[index];
    }

    public void OnHover(HandTargetEventData eventData)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UseDecoyTube", true, GameInput.Button.LeftHand);
        main.SetText(HandReticle.TextType.HandSubscript, string.Empty, false, GameInput.Button.None);
        main.SetIcon(HandReticle.IconType.Hand, 1f);
    }

    public void OnUse(HandTargetEventData eventData)
    {
        PDA pda = Player.main.GetPDA();
        Inventory.main.SetUsedStorage(equipment);
        pda.Open(PDATab.Inventory);
    }

    private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
    {
        return AllowedPowerSources.Keys.Contains(pickupable.GetTechType());
    }
}
