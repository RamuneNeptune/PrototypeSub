using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.HydroVentilators;

internal class ProtoHydrodynamicVentilators : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

    private bool upgradeActive;
    private float[] originalMotorSpeeds;

    private void Start()
    {
        originalMotorSpeeds = motorMode.motorModeSpeeds;
    }

    private void FixedUpdate()
    {
        if (!upgradeActive) return;

        float depth = crushDamage.GetDepth();

        if (depth < activationDepth)
        {
            motorMode.motorModeSpeeds = originalMotorSpeeds;
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        float[] newPowerValues = new float[originalMotorSpeeds.Length];
        for (int i = 0; i < newPowerValues.Length; i++)
        {
            newPowerValues[i] = originalMotorSpeeds[i] * multiplier;
        }

        motorMode.motorModeSpeeds = newPowerValues;
    }

    public void SetUpgradeActive(bool active)
    {
        upgradeActive = active;
    }
}
