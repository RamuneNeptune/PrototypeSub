using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerBattery : MonoBehaviour, IBattery, ISaveDataListener, ILateSaveDataListener
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
    private PrototypeSaveData protoSaveData;

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
        float power = PrototypePowerSystem.AllowedPowerSources[techTag.type];
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

    public void OnLateSaveDataLoaded(BaseSubDataClass saveData)
    {
        Initialize();

        protoSaveData = saveData.EnsureAsPrototypeData();
        if (protoSaveData.normalizedBatteryCharges.TryGetValue(prefabIdentifier.Id, out float normalizedCharge))
        {
            charge = capacity * normalizedCharge;
        }
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData) { }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var data = saveData.EnsureAsPrototypeData();
        if (connectedBattery != null) return;

        if (!data.normalizedBatteryCharges.ContainsKey(prefabIdentifier.Id))
        {
            data.normalizedBatteryCharges.Add(prefabIdentifier.Id, charge / capacity);
        }
        else
        {
            data.normalizedBatteryCharges[prefabIdentifier.Id] = charge / capacity;
        }
    }

    private void OnDestroy()
    {
        Initialize(); //Make sure the prefab identifier is set if for some reason nothing else is called

        Plugin.Logger.LogInfo($"Batt = {gameObject} | Save data = {protoSaveData} | Identifier = {prefabIdentifier} | Charges = {protoSaveData?.normalizedBatteryCharges}");
        protoSaveData.normalizedBatteryCharges.Remove(prefabIdentifier.Id);
    }
}
