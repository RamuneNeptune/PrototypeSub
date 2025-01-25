using Nautilus.Json;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerBattery : MonoBehaviour, IBattery, IProtoTreeEventListener
{
    public float charge
    {
        get
        {
            return _charge;
        }
        set
        {
            _charge = Mathf.Min(value, _capacity);
        }
    }
    public float capacity
    {
        get
        {
            return _capacity;
        }
        set
        {
            _capacity = value;
        }
    }

    private float BatteryCapacityRatio
    {
        get
        {
            if (connectedBattery == null) return -1;

            return connectedBattery.capacity / capacity;
        }
    }

    public InventoryItem InventoryItem
    {
        get
        {
            if (pickupable == null) pickupable = GetComponent<Pickupable>();

            if (pickupable.inventoryItem == null)
            {
                pickupable.inventoryItem = new InventoryItem(pickupable);
            }

            return pickupable.inventoryItem;
        }
    }

    private IBattery connectedBattery;
    private float lastBatteryCharge;
    private Pickupable pickupable;
    private PrefabIdentifier prefabIdentifier;

    private float _charge;
    private float _capacity;
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;

        pickupable = GetComponent<Pickupable>();
        prefabIdentifier = GetComponent<PrefabIdentifier>();
        connectedBattery = GetComponents<IBattery>().FirstOrDefault(i => i != (IBattery)this);

        var techTag = GetComponent<TechTag>();
        if (techTag == null || !PrototypePowerSystem.AllowedPowerSources.ContainsKey(techTag.type))
        {
            initialized = true;
            string ttName = techTag != null ? techTag.type.ToString() : "Null tech tag";
            Plugin.Logger.LogWarning($"Prototype battery on {gameObject} with invalid tech type ({ttName}). Destroying battery component.");
            Destroy(this);
            return;
        }

        float power = PrototypePowerSystem.AllowedPowerSources[techTag.type].powerValue;
        SetCapacity(power);
        SetCharge(power);

        TryMatchBatteryCharge();

        initialized = true;
    }

    public void SetCapacity(float capacity)
    {
        this.capacity = capacity;
    }

    public void SetCharge(float charge, bool updateConnectedBattery = false)
    {
        this.charge = charge;

        if (updateConnectedBattery && connectedBattery != null)
        {
            connectedBattery.charge = this.charge * BatteryCapacityRatio;
        }

        if (charge <= 0)
        {
            InventoryItem.container.RemoveItem(InventoryItem, true, false);
            Destroy(gameObject);
        }
    }

    public void SetChargeNormalized(float normalizedCharge, bool updateConnectedBattery = false)
    {
        charge = capacity * normalizedCharge;

        if (updateConnectedBattery && connectedBattery != null)
        {
            connectedBattery.charge = charge * BatteryCapacityRatio;
        }
    }

    public void TryMatchBatteryCharge()
    {
        if (connectedBattery == null) return;

        charge = connectedBattery.charge * (1 / BatteryCapacityRatio);
    }

    public void ModifyCharge(float change)
    {
        SetCharge(charge + change, true);
    }

    private void Update()
    {
        if (connectedBattery == null) return;

        if (lastBatteryCharge != charge)
        {
            connectedBattery.charge = charge * BatteryCapacityRatio;
        }

        lastBatteryCharge = charge;
    }

    public string GetChargeValueText()
    {
        float num = charge / capacity;
        return Language.main.GetFormat("BatteryCharge", num, Mathf.RoundToInt(charge), capacity);
    }

    public void OnBeforeDataSaved(object sender, JsonFileEventArgs args)
    {
        if (connectedBattery != null) return;

        var data = Plugin.GlobalSaveData;
        if (!data.normalizedBatteryCharges.ContainsKey(prefabIdentifier.Id))
        {
            data.normalizedBatteryCharges.Add(prefabIdentifier.Id, charge / capacity);
        }
        else
        {
            data.normalizedBatteryCharges[prefabIdentifier.Id] = charge / capacity;
        }
    }

    private void OnEnable() => Plugin.GlobalSaveData.OnStartedSaving += OnBeforeDataSaved;
    private void OnDisable() => Plugin.GlobalSaveData.OnStartedSaving -= OnBeforeDataSaved;

    private void OnDestroy()
    {
        Plugin.GlobalSaveData.normalizedBatteryCharges.Remove(prefabIdentifier.Id);
    }

    public void OnProtoSerializeObjectTree(ProtobufSerializer serializer) { }
    public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
    {
        Initialize();

        if (Plugin.GlobalSaveData.normalizedBatteryCharges.TryGetValue(prefabIdentifier.Id, out float normalizedCharge))
        {
            charge = capacity * normalizedCharge;
        }
    }
}
