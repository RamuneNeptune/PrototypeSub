using UnityEngine;

namespace PrototypeSubMod.MotorHandler;

internal class ProtoMotorHandler : MonoBehaviour
{
    [SerializeField] private CyclopsMotorMode motorMode;

    private bool allowedToMove;
    private float[] originalMotorSpeeds;
    private float[] originalPowerValues;

    private float originalTurningTorque;
    private float speedMultiplier = 1f;
    private float speedBonus;
    private float powerMultiplier = 1f;
    private float overrideNoiseValue = -1;

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

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    /// <summary>
    /// Adds the given speed parameter to the existing speed multiplier
    /// </summary>
    /// <param name="extraSpeed">How much extra speed to add</param>
    public void SetSpeedMultiplierBonus(float extraSpeed)
    {
        speedBonus = extraSpeed;
    }

    /// <summary>
    /// Set this to -1 if you don't want to change it
    /// </summary>
    /// <param name="noiseValue"></param>
    public void SetOverrideNoiseValue(float noiseValue)
    {
        overrideNoiseValue = noiseValue;
    }

    public float GetOverrideNoiseValue()
    {
        return overrideNoiseValue;
    }

    public void SetPowerMultiplier(float multiplier)
    {
        powerMultiplier = multiplier;
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
}
