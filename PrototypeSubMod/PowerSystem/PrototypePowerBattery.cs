using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerBattery : MonoBehaviour
{
    public float Charge { get; private set; }
    public float Capacity { get; private set; }

    private float BatteryCapacityRatio
    {
        get
        {
            if (battery == null) return -1;

            return battery.capacity / Capacity;
        }
    }

    private IBattery battery;
    private float lastBatteryCharge;

    private void Awake()
    {
        battery = GetComponent<IBattery>();

        var techTag = GetComponent<TechTag>();
        float power = PrototypePowerSystem.AllowedPowerSources[techTag.type];
        SetCharge(power);
        SetCapacity(power);

        TryMatchBatteryCharge();
    }

    public void SetCapacity(float capacity)
    {
        Capacity = capacity;
    }

    public void SetCharge(float charge, bool updateConnectedBattery = false)
    {
        Charge = charge;

        if (updateConnectedBattery && battery != null)
        {
            battery.charge = charge * BatteryCapacityRatio;
        }
    }

    public void TryMatchBatteryCharge()
    {
        if (battery == null) return;

        Charge = battery.charge * (1 / BatteryCapacityRatio);
    }

    public void ModifyCharge(float change)
    {
        SetCharge(Charge + change, true);
    }

    private void Update()
    {
        if (battery == null) return;

        if (lastBatteryCharge != Charge)
        {
            battery.charge = Charge * BatteryCapacityRatio;
        }

        lastBatteryCharge = Charge;
    }
}
