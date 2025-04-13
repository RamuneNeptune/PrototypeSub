using PrototypeSubMod.PowerSystem.Funcionalities;
using PrototypeSubMod.Prefabs;
using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using PrototypeSubMod.Prefabs.AlienBuildingBlock;
using UnityEngine;
using Random = UnityEngine.Random;

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
        { WarperRemnant.prefabInfo.TechType, new(2, null) },
        { AlienBuildingBlock.prefabInfo.TechType, new(4, null) },
        { TechType.PrecursorIonCrystal, new(5, typeof(IonCubePowerFunctionality)) },
        { TechType.PrecursorIonCrystalMatrix, new(8, null) },
        { IonPrism_Craftable.prefabInfo.TechType, new(10, null) }
    };

    public const float CHARGE_POWER_AMOUNT = 200f;

    internal static readonly string EquipmentLabel = "PrototypePowerLabel";

    public Equipment equipment { get; private set; }
    public event Action onReorderSources;

    [SerializeField] private SubSerializationManager serializationManager;
    [SerializeField] private ChildObjectIdentifier storageRoot;
    [SerializeField] private PrototypePowerSource[] batterySources;
    [SerializeField] private ProtoPowerRelay[] powerRelays;
    [SerializeField] private FMOD_CustomLoopingEmitter ambientSFX;

    private Coroutine relayActivatorCoroutine;
    private bool reorderingItems;
    
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

        UpdateAmbientSFX();
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
        
        foreach (var relay in powerRelays)
        {
            relay.SetRelayActive(false);
        }
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
        UpdateAmbientSFX();
    }

    private void OnUnequip(string slot, InventoryItem item)
    {
        int index = Array.IndexOf(SLOT_NAMES, slot);

        var batterySource = batterySources[index];
        batterySource.SetBattery(null);
        UpdateAmbientSFX();
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

    public PrototypePowerSource[] GetPowerSources() => batterySources;
    
    private void OnRemoveItem(InventoryItem item)
    {
        if (reorderingItems) return;
        
        UWE.CoroutineHost.StartCoroutine(OnRemoveItemDelayed());
    }

    private IEnumerator OnRemoveItemDelayed()
    {
        reorderingItems = true;
        yield return new WaitForEndOfFrame();
        
        // Items will only be removed via the consumption of an item, or the removal of one
        List<InventoryItem> newItems = new();
        for (int i = 0; i < SLOT_NAMES.Length; i++)
        {
            string slot = SLOT_NAMES[i];
            var itemInSlot = equipment.GetItemInSlot(slot);
            if (itemInSlot == null) continue;

            newItems.Add(itemInSlot);
            equipment.RemoveItem(slot, false, false);
        }

        for (int i = 0; i < newItems.Count; i++)
        {
            equipment.AddItem(SLOT_NAMES[i], newItems[i]);
        }

        UpdateRelayStatus();
        UpdateAmbientSFX();
        reorderingItems = false;

        onReorderSources?.Invoke();
    }

    private void UpdateRelayStatus()
    {
        List<ProtoPowerRelay> activeRelays = new();
        
        for (int i = 0; i < powerRelays.Length; i++)
        {
            if (i >= SLOT_NAMES.Length) break;
            
            var relay = powerRelays[i];
            var itemInSlot = equipment.GetItemInSlot(SLOT_NAMES[i]);
            relay.SetPowerSource(itemInSlot);
            bool active = itemInSlot != null;
            if (active)
            {
                activeRelays.Add(relay);
            }
            else
            {
                relay.SetRelayActive(false);
            }
        }

        if (relayActivatorCoroutine != null) UWE.CoroutineHost.StopCoroutine(relayActivatorCoroutine);
        relayActivatorCoroutine = UWE.CoroutineHost.StartCoroutine(UpdateActiveRelays(activeRelays));
    }

    private IEnumerator UpdateActiveRelays(List<ProtoPowerRelay> relays)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        for (int i = 0; i < relays.Count; i++)
        {
            var relay = relays[i];
            relay.SetRelayActive(true);
            
            var nextRelay = relays[Mathf.Min(i + 1, relays.Count - 1)];
            if (!nextRelay.GetRelayActive())
            {
                yield return new WaitForSeconds(Random.Range(0f, 1f));
            }
        }
    }

    private void UpdateAmbientSFX()
    {
        bool active = equipment.equipment.Values.Any(i => i != null);
        if (active)
        {
            ambientSFX.Play();
        }
        else
        {
            ambientSFX.Stop();
        }
    }

    private void OnDisable()
    {
        ambientSFX.Stop(STOP_MODE.IMMEDIATE);
    }

    private void OnEnable()
    {
        UpdateAmbientSFX();
    }
}
