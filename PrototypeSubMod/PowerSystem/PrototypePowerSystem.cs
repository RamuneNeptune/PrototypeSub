using PrototypeSubMod.PowerSystem.Funcionalities;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

public class PrototypePowerSystem : MonoBehaviour, ISaveDataListener, IProtoTreeEventListener
{
    internal static readonly string[] SLOT_NAMES = new string[]
    {
        "PrototypePowerSlot1",
        "PrototypePowerSlot2",
        "PrototypePowerSlot3",
        "PrototypePowerSlot4"
    };

    internal static readonly Dictionary<TechType, PowerConfigData> AllowedPowerSources = new()
    {
        { TechType.PrecursorIonCrystal, new(1000, typeof(IonCubePowerFunctionality)) },
        { TechType.PrecursorIonPowerCell, new(1500, typeof(IonPowerCellFunctionality)) },
        { TechType.PrecursorIonCrystalMatrix, new(3500, null) },
        { IonPrism_Craftable.prefabInfo.TechType, new(5000, null) }
    };

    internal static readonly string EquipmentLabel = "PrototypePowerLabel";

    public Equipment equipment { get; private set; }

    [SerializeField] private SubSerializationManager serializationManager;
    [SerializeField] private ChildObjectIdentifier storageRoot;
    [SerializeField] private PrototypePowerSource[] batterySources;
    [SerializeField] private ProtoPowerRelay[] powerRelays;
    
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
        equipment.onRemoveItem += OnRemoveItem;

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
        UpdateRelayStatus();
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        int index = Array.IndexOf(SLOT_NAMES, slot);

        var batterySource = batterySources[index];
        batterySource.SetBattery(null);
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

    public static void AddPowerSource(TechType techType, PowerConfigData configData)
    {
        if (AllowedPowerSources.ContainsKey(techType)) return;

        AllowedPowerSources.Add(techType, configData);
    }

    public bool StorageSlotsFull()
    {
        return storageRoot.transform.childCount >= SLOT_NAMES.Length;
    }
    
    private void OnRemoveItem(InventoryItem item)
    {
        // Items will only be removed via the consumption of an item, or the removal of one
        int firstItemIndex = int.MaxValue;
        List<InventoryItem> newItems = new();
        for (int i = SLOT_NAMES.Length - 1; i >= 0; i--)
        {
            string slot = SLOT_NAMES[i];
            var itemInSlot = equipment.GetItemInSlot(slot);
            if (itemInSlot == null && firstItemIndex != int.MaxValue) continue;
            
            if (firstItemIndex == int.MaxValue) firstItemIndex = i;

            newItems.Insert(0, itemInSlot);
            equipment.RemoveItem(slot, false, false);
        }

        for (int i = 0; i < newItems.Count; i++)
        {
            equipment.AddItem(SLOT_NAMES[i], newItems[i]);
        }

        UpdateRelayStatus();
    }

    private void UpdateRelayStatus()
    {
        for (int i = 0; i < powerRelays.Length; i++)
        {
            var relay = powerRelays[i];
            bool active = (i + 1) < storageRoot.transform.childCount;
            relay.SetRelayActive(active);
            relay.SetSourceType(equipment.GetItemInSlot(SLOT_NAMES[i]).techType);
        }
    }
}
