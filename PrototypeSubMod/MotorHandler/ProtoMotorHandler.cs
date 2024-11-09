using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.MotorHandler;

internal class ProtoMotorHandler : MonoBehaviour
{
    [SerializeField] private CyclopsMotorMode motorMode;

    private bool allowedToMove;
    private float[] originalMotorSpeeds;
    private float[] originalPowerValues;
    private float originalTurningTorque;

    private Dictionary<Component, float> speedMultipliers;
    private Dictionary<Component, float> speedBonuses;
    private Dictionary<Component, float> powerEfficiencyMultipliers;
    private Dictionary<Component, float> overrideNoiseValues;

    private void Start()
    {
        originalTurningTorque = motorMode.subController.BaseTurningTorque;
        originalMotorSpeeds = motorMode.motorModeSpeeds;
        originalPowerValues = motorMode.motorModePowerConsumption;
    }

    public void SetAllowedToMove(bool allowedToMove)
    {
        this.allowedToMove = allowedToMove;
    }

    public void AddSpeedMultiplier(ValueRegistrar speedMultiplier)
    {
        if (speedMultipliers.ContainsKey(speedMultiplier.component))
        {
            speedMultipliers[speedMultiplier.component] = speedMultiplier.value;
            return;
        }

        speedMultipliers.Add(speedMultiplier.component, speedMultiplier.value);
    }

    public bool RemoveSpeedMultiplier(Component component)
    {
        if (!speedMultipliers.ContainsKey(component)) return false;

        speedMultipliers.Remove(component);
        return true;
    }

    /// <summary>
    /// Adds the given speed parameter to the existing speed multiplier
    /// </summary>
    /// <param name="extraSpeed"></param>
    public void AddSpeedMultiplierBonus(ValueRegistrar multiplierBonus)
    {
        if (speedBonuses.ContainsKey(multiplierBonus.component))
        {
            speedBonuses[multiplierBonus.component] = multiplierBonus.value;
            return;
        }

        speedBonuses.Add(multiplierBonus.component, multiplierBonus.value);
    }

    public bool RemoveSpeedMultiplierBonus(Component component)
    {
        if (!speedBonuses.ContainsKey(component)) return false;

        speedBonuses.Remove(component);
        return true;
    }

    /// <summary>
    /// Set this to -1 if you don't want to change it
    /// </summary>
    /// <param name="noiseValue"></param>
    public void AddOverrideNoiseValue(ValueRegistrar overrideValue)
    {
        if (overrideNoiseValues.ContainsKey(overrideValue.component))
        {
            overrideNoiseValues[overrideValue.component] = overrideValue.value;
            return;
        }

        overrideNoiseValues.Add(overrideValue.component, overrideValue.value);
    }

    public bool RemoveOverrideNoiseValue(Component component)
    {
        if (!overrideNoiseValues.ContainsKey(component)) return false;

        overrideNoiseValues.Remove(component);
        return true;
    }

    /// <summary>
    /// Controls the power efficiency of the engine. Higher number = more efficient. 1 = default
    /// </summary>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public void AddPowerEfficiencyMultiplier(ValueRegistrar efficiencyMultiplier)
    {
        if (powerEfficiencyMultipliers.ContainsKey(efficiencyMultiplier.component))
        {
            powerEfficiencyMultipliers[efficiencyMultiplier.component] = efficiencyMultiplier.value;
            return;
        }

        powerEfficiencyMultipliers.Add(efficiencyMultiplier.component, efficiencyMultiplier.value);
    }

    public bool RemovePowerEfficiencyMultiplier(Component component)
    {
        if (!powerEfficiencyMultipliers.ContainsKey(component)) return false;

        powerEfficiencyMultipliers.Remove(component);
        return true;
    }

    public float GetEfficiencyMultiplier()
    {
        float total = 1;
        foreach (var registrar in powerEfficiencyMultipliers)
        {
            total *= registrar.Value;
        }

        return total;
    }

    public float GetOverrideNoiseValue()
    {
        if (overrideNoiseValues.Count == 0) return -1;

        float total = 0;
        foreach (var item in overrideNoiseValues)
        {
            total += item.Value;
        }

        return total;
    }

    private void Update()
    {
        if (!allowedToMove)
        {
            motorMode.motorModeSpeeds = new float[3];
            motorMode.ChangeCyclopsMotorMode(motorMode.cyclopsMotorMode);
            motorMode.subController.BaseTurningTorque = 0;
            return;
        }

        float speedMultiplier = 1f;
        foreach (var item in speedMultipliers)
        {
            speedMultiplier *= item.Value;
        }

        float speedBonus = 0f;
        foreach (var item in speedBonuses)
        {
            speedBonus += item.Value;
        }

        float[] newSpeeds = originalMotorSpeeds;
        newSpeeds.ForEach(s => s *= speedMultiplier + speedBonus);

        if (motorMode.motorModeSpeeds != newSpeeds)
        {
            motorMode.motorModeSpeeds = newSpeeds;
            motorMode.ChangeCyclopsMotorMode(motorMode.cyclopsMotorMode);
            motorMode.subController.BaseTurningTorque = originalTurningTorque;
        }
    }

    public bool GetAllowedToMove()
    {
        return allowedToMove;
    }

    public struct ValueRegistrar
    {
        public Component component;
        public float value;

        /// <summary>
        /// Creates a new value registrar
        /// </summary>
        /// <param name="component">The component registering the value</param>
        /// <param name="value">The value to register</param>
        public ValueRegistrar(Component component, float value)
        {
            this.component = component;
            this.value = value;
        }
    }
}
