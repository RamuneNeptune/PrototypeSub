using PrototypeSubMod.SaveData;
using SubLibrary.SaveData;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerSource : MonoBehaviour, IPowerInterface, ISaveDataListener
{
    public float Charge
    {
        get
        {
            float num = battery != null && !ElectronicsDisabled ? battery.charge : 0;
            if (!ElectronicsDisabled && Time.time < enableElectronicsTime + 2f)
            {
                num *= Mathf.InverseLerp(enableElectronicsTime, enableElectronicsTime + 2f, Time.time);
            }

            return num;
        }
    }

    public float Capacity
    {
        get
        {
            if (battery != null) return battery.capacity;

            return 0;
        }
    }

    public bool ElectronicsDisabled
    {
        get
        {
            return _electronicsDisabled;
        }
        private set
        {
            if (value == _electronicsDisabled) return;

            _electronicsDisabled = value;
        }
    }

    [SerializeField] private TechType defaultBattery;
    [SerializeField, Range(0, 1)] private float defaultBatteryCharge;
    [SerializeField] private ChildObjectIdentifier storageRoot;
    [SerializeField] private PrototypePowerSystem powerSystem;

    private PrototypeSaveData protoSaveData;
    private PrototypeSaveData.PowerSourceData powerSourceData;

    private PrototypePowerBattery battery;
    private PowerRelay connectedRelay;

    private float enableElectronicsTime;
    private bool _electronicsDisabled;
    private bool defaultBatteryCreated;
    private bool allowedToPlaySounds = true;

    private void Start()
    {
        UpdateConnection();
        InvokeRepeating(nameof(UpdateConnection), UnityEngine.Random.value, 1f);

        if (protoSaveData == null && !powerSourceData.defaultBatteryCreated)
        {
            CoroutineHost.StartCoroutine(SpawnDefaultBattery());
        }
    }

    #region IPowerInterface

    public bool GetInboundHasSource(IPowerInterface powerInterface)
    {
        //We don't have any inbound power sources
        return false;
    }

    public float GetMaxPower()
    {
        return Capacity;
    }

    public float GetPower()
    {
        return Charge;
    }

    public bool HasInboundPower(IPowerInterface powerInterface)
    {
        return false;
    }

    public bool ModifyPower(float amount, out float modified)
    {
        float chargeChange = 0;
        modified = chargeChange;

        if (!GameModeUtils.RequiresPower() || ElectronicsDisabled) return false;

        if (battery == null) return false;
        
        if (amount > 0)
        {
            chargeChange = Mathf.Min(amount, Capacity - Charge);
        }
        else
        {
            chargeChange = GetChargeChangeSubtract(amount);
        }
        
        battery.ModifyCharge(chargeChange);

        modified = chargeChange;
        
        // Returns whether the amount drawn was less than the charge in the battery
        return amount >= 0f ? amount <= Capacity - Charge : Charge > -amount;
    }

    private float GetChargeChangeSubtract(float amount)
    {
        float chargeChange;
        
        int incrementCount = Mathf.FloorToInt(amount / PrototypePowerSystem.CHARGE_POWER_AMOUNT);
        float chargeRemainder = -(amount / PrototypePowerSystem.CHARGE_POWER_AMOUNT - incrementCount) * PrototypePowerSystem.CHARGE_POWER_AMOUNT;
        
        float mod = Charge % PrototypePowerSystem.CHARGE_POWER_AMOUNT;
        bool exceedsCharge = (mod != 0 && mod + amount < 0) || (mod == 0 && -amount > PrototypePowerSystem.CHARGE_POWER_AMOUNT);
        
        if (!exceedsCharge)
        {
            chargeChange = amount;
        }
        else if (Charge + amount > 0)
        {
            chargeChange = chargeRemainder != 0 ? Mathf.Max(amount - chargeRemainder, -mod) : -Mathf.Min(-amount, PrototypePowerSystem.CHARGE_POWER_AMOUNT);
        }
        else
        {
            chargeChange = amount + Charge;
        }

        return chargeChange;
    }

    #endregion

    //Note: Add this to EMPBlast.OnTouch()
    public void DisableElectronicsForTime(float time)
    {
        enableElectronicsTime = Mathf.Max(enableElectronicsTime, Time.time + time);
        ElectronicsDisabled = true;
    }

    private void UpdateConnection()
    {
        var relay = PowerSource.FindRelay(transform);
        if (relay && relay != connectedRelay)
        {
            if (connectedRelay != null)
            {
                connectedRelay.RemoveInboundPower(this);
            }

            connectedRelay = relay;
            connectedRelay.AddInboundPower(this);
        }
    }

    public void OnSaveDataLoaded(BaseSubDataClass saveData)
    {
        protoSaveData = saveData.EnsureAsPrototypeData();

        if (protoSaveData.powerSourceDatas == null)
        {
            protoSaveData.powerSourceDatas = new();
        }

        if (!protoSaveData.powerSourceDatas.ContainsKey(gameObject.name))
        {
            protoSaveData.powerSourceDatas.Add(gameObject.name, new PrototypeSaveData.PowerSourceData());
        }

        powerSourceData = protoSaveData.powerSourceDatas[gameObject.name];
        if (!powerSourceData.defaultBatteryCreated)
        {
            CoroutineHost.StartCoroutine(SpawnDefaultBattery());
        }
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        var protoData = saveData.EnsureAsPrototypeData();

        protoData.powerSourceDatas[gameObject.name] = powerSourceData;
    }

    public IEnumerator SpawnDefaultBattery()
    {
        CoroutineTask<GameObject> prefabTask = CraftData.GetPrefabForTechTypeAsync(defaultBattery);

        yield return prefabTask;

        GameObject prefab = prefabTask.result.Get();
        var instantiatedPrefab = Instantiate(prefab, storageRoot.transform);
        instantiatedPrefab.SetActive(false);

        var battery = instantiatedPrefab.GetComponent<PrototypePowerBattery>();
        this.battery = battery;

        battery.SetChargeNormalized(defaultBatteryCharge);

        string slot = PrototypePowerSystem.SLOT_NAMES[transform.GetSiblingIndex()];

        powerSystem.equipment.AddItem(slot, battery.InventoryItem);

        powerSourceData.defaultBatteryCreated = true;
    }

    public void SetBattery(PrototypePowerBattery battery)
    {
        this.battery = battery;

        string slot = PrototypePowerSystem.SLOT_NAMES[transform.GetSiblingIndex()];

        if (battery == null)
        {
            powerSystem.equipment.RemoveItem(slot, false, false);
        }
        else
        {
            battery.Initialize();
            powerSystem.equipment.AddItem(slot, battery.InventoryItem);
        }
    }

    private void PlayBatterySound(bool hasBattery)
    {
        Plugin.Logger.LogError($"Battery sounds not yet implemented!");
    }
}
