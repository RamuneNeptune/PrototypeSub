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
            if (battery != null && battery.charge > 0)
            {
                NotifyPowered(_electronicsDisabled);
                PlayPowerSound(_electronicsDisabled);
            }
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

        if(protoSaveData == null && !powerSourceData.defaultBatteryCreated)
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
        modified = amount;

        if (!GameModeUtils.RequiresPower() || ElectronicsDisabled) return false;

        float oldCharge = Charge;
        if (battery != null)
        {
            if (amount >= 0f)
            {
                chargeChange = Mathf.Min(amount, battery.capacity - battery.charge);
            }
            else
            {
                chargeChange = -Mathf.Min(-amount, battery.charge);
            }

            battery.ModifyCharge(chargeChange);
        }

        if (oldCharge == 0f && Charge > 0f)
        {
            NotifyPowered(true);
            PlayPowerSound(true);
        }
        else if (oldCharge > 0f && Charge == 0f)
        {
            NotifyPowered(false);
            PlayPowerSound(false);
        }

        modified = chargeChange;

        //Tbh I have know idea why this is needed. It returns whether the delta would have exceeded the limits of the source,
        //but it's clamped anyway. Idk.
        return amount >= 0f ? amount <= Capacity - Charge : Charge > -amount;
    }

    private void NotifyPowered(bool powered)
    {
        Plugin.Logger.LogError($"Notify powered not yet implemented!");
    }

    private void PlayPowerSound(bool powered)
    {
        Plugin.Logger.LogError($"Play power sound not yet implemented!");
    }

    #endregion

    //Note: add this to EMPBlast.OnTouch()
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

        //defaultBatteryCreated will be set here
        protoSaveData = saveData as PrototypeSaveData;

        if (!protoSaveData.powerSourceDatas.ContainsKey(gameObject.name))
        {
            protoSaveData.powerSourceDatas.Add(gameObject.name, new PrototypeSaveData.PowerSourceData());
        }

        powerSourceData = protoSaveData.powerSourceDatas[gameObject.name];
        if(!powerSourceData.defaultBatteryCreated)
        {
            CoroutineHost.StartCoroutine(SpawnDefaultBattery());
        }
    }

    public void OnBeforeDataSaved(ref BaseSubDataClass saveData)
    {
        if(saveData is not PrototypeSaveData)
        {
            //This is the first time saving and the data type is ModuleSaveData
            saveData = new PrototypeSaveData();
        }

        (saveData as PrototypeSaveData).powerSourceDatas[gameObject.name] = powerSourceData;
    }

    private IEnumerator SpawnDefaultBattery()
    {
        CoroutineTask<GameObject> prefabTask = CraftData.GetPrefabForTechTypeAsync(defaultBattery);

        yield return prefabTask;

        GameObject prefab = prefabTask.result.Get();
        var instantiatedPrefab = Instantiate(prefab, storageRoot.transform);
        instantiatedPrefab.SetActive(false);

        var battery = instantiatedPrefab.GetComponent<PrototypePowerBattery>();
        this.battery = battery;

        battery.SetChargeNormalized(defaultBatteryCharge);  

        string slot = PrototypePowerSystem.SLOT_NAMES[transform.GetSiblingIndex() - 1];

        powerSystem.equipment.AddItem(slot, battery.InventoryItem);

        powerSourceData.defaultBatteryCreated = true;
    }

    public void SetBattery(PrototypePowerBattery battery)
    {
        this.battery = battery;
        battery.Initialize();

        string slot = PrototypePowerSystem.SLOT_NAMES[transform.GetSiblingIndex() - 1];

        if(battery == null)
        {
            powerSystem.equipment.RemoveItem(slot, false, false);
        }
        else
        {
            powerSystem.equipment.AddItem(slot, battery.InventoryItem);
        }
    }

    private void PlayBatterySound(bool hasBattery)
    {
        Plugin.Logger.LogError($"Battery sounds not yet implemented!");
    }
}
