using PrototypeSubMod.Interfaces;
using PrototypeSubMod.MotorHandler;
using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

internal class ProtoPressureConverters : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

    private bool convertersActive;

    private void FixedUpdate()
    {
        if (!convertersActive) return;

        float depth = crushDamage.GetDepth();

        if (depth < activationDepth)
        {
            motorHandler.SetPowerMultiplier(1f);
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        motorHandler.SetPowerMultiplier(multiplier);
    }

    public void SetUpgradeInstalled(bool active)
    {
        convertersActive = active;
    }

    public bool GetUpgradeInstalled() => convertersActive;

    public string GetUpgradeName()
    {
        return "Pressure Converters";
    }
}
