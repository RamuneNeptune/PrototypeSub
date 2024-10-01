using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerSystem : MonoBehaviour, ISaveDataListener, IProtoTreeEventListener
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
        { TechType.PrecursorIonCrystal, 1000 },
        { TechType.PrecursorIonCrystalMatrix, 3000 },
        { TechType.PrecursorIonPowerCell, 4000 },
    };

    public static readonly string EquipmentLabel = "PrototypePowerLabel";

    public Equipment equipment { get; private set; }

    [SerializeField] private SubSerializationManager serializationManager;
    [SerializeField] private ChildObjectIdentifier storageRoot;
    [SerializeField] private PrototypePowerSource[] batterySources;
    [SerializeField] private FMODAsset equipBatterySound;
    [SerializeField] private FMODAsset unequipBatterySound;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        if (batterySources.Length != SLOT_NAMES.Length)
        {
            Plugin.Logger.LogError($"Battery source and slot name length mismatch on {gameObject}!");
        }
    }

    private void Initialize()
    {
        if (equipment != null) return;

        equipment = new(gameObject, storageRoot.transform);
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
        int index = Array.IndexOf(SLOT_NAMES, slot);

        var batterySource = batterySources[index];

        if (!item.item.TryGetComponent(out PrototypePowerBattery battery))
        {
            Plugin.Logger.LogError($"Item ({item}) added to prototype power system doesn't have a PrototypePowerBattery component on it!");
            return;
        }

        batterySource.SetBattery(battery);

        FMODUWE.PlayOneShot(equipBatterySound, transform.position, 1f);
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        int index = Array.IndexOf(SLOT_NAMES, slot);

        var batterySource = batterySources[index];
        batterySource.SetBattery(null);

        FMODUWE.PlayOneShot(unequipBatterySound, transform.position, 1f);
    }

    public void OnHover(HandTargetEventData eventData)
    {
        HandReticle main = HandReticle.main;
        main.SetText(HandReticle.TextType.Hand, "UseProtoPowerSystem", true, GameInput.Button.LeftHand);
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

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        Initialize();
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();
        protoData.serializedPowerEquipment = equipment.SaveEquipment();

        saveData = protoData;
    }

    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer) { }

    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        var data = serializationManager.saveData.EnsureAsPrototypeData();
        if (data.serializedPowerEquipment != null)
        {
            StorageHelper.TransferEquipment(storageRoot.gameObject, data.serializedPowerEquipment, equipment);
        }
    }
}
