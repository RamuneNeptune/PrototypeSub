using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerBattery : MonoBehaviour, IBattery
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

    private IBattery connectedBattery;
    private float lastBatteryCharge;

    private float _charge;
    private float _capacity;

    private void Awake()
    {
        connectedBattery = GetComponent<IBattery>();
        if (connectedBattery == (IBattery)this) connectedBattery = null;

        var techTag = GetComponent<TechTag>();
        float power = PrototypePowerSystem.AllowedPowerSources[techTag.type];
        SetCapacity(power);
        SetCharge(power);

        TryMatchBatteryCharge();
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
}
