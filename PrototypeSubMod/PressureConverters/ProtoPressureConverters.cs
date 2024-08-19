using PrototypeSubMod.Interfaces;
using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

internal class ProtoPressureConverters : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

    private bool convertersActive;
    private float[] originalPowerValues;

    private void Start()
    {
        originalPowerValues = motorMode.motorModePowerConsumption;
    }

    private void FixedUpdate()
    {
        if (!convertersActive) return;

        float depth = crushDamage.GetDepth();

        if(depth < activationDepth)
        {
            motorMode.motorModePowerConsumption = originalPowerValues;
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        float[] newPowerValues = new float[originalPowerValues.Length];
        for (int i = 0; i < newPowerValues.Length; i++)
        {
            newPowerValues[i] = originalPowerValues[i] * multiplier;
        }

        motorMode.motorModePowerConsumption = newPowerValues;
    }

    public void SetUpgradeActive(bool active)
    {
        convertersActive = active;
    }

    public bool GetUpgradeActive() => convertersActive;
}
