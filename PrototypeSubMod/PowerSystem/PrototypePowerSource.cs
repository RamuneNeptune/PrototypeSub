using System;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem;

internal class PrototypePowerSource : MonoBehaviour, IPowerInterface
{
    public float Charge
    {
        get
        {
            float num = battery != null && !ElectronicsDisabled ? battery.Charge : 0;
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
            if (battery != null) return battery.Capacity;

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
            if (battery != null && battery.Charge > 0)
            {
                NotifyPowered(_electronicsDisabled);
                PlayPowerSound(_electronicsDisabled);
            }
        }
    }

    private PrototypePowerBattery battery;
    private PowerRelay connectedRelay;

    private float enableElectronicsTime;
    private bool _electronicsDisabled;

    private void Start()
    {
        UpdateConnection();
        InvokeRepeating(nameof(UpdateConnection), UnityEngine.Random.value, 1f);
    }

    public bool GetInboundHasSource(IPowerInterface powerInterface)
    {
        //We don't have any inbound power sources
        return false;
    }

    public void SetBattery(PrototypePowerBattery battery)
    {
        this.battery = battery;
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
                chargeChange = Mathf.Min(amount, battery.Capacity - battery.Charge);
            }
            else
            {
                chargeChange = -Mathf.Min(-amount, battery.Charge);
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
        throw new NotImplementedException();
    }

    private void PlayPowerSound(bool powered)
    {
        throw new NotImplementedException();
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
}
