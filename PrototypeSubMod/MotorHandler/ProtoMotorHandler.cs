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
    private float powerMultiplier = 1f;

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
        newSpeeds.ForEach(s => s *= speedMultiplier);

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
