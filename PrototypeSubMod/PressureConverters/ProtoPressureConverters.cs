using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

internal class ProtoPressureConverters : MonoBehaviour
{
    [SerializeField] private CyclopsMotorMode motorMode;
    [SerializeField] private CrushDamage crushDamage;

    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

    private float[] originalPowerValues;

    private void Start()
    {
        originalPowerValues = motorMode.motorModePowerConsumption;
    }

    private void FixedUpdate()
    {
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
}
