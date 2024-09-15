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

    private bool convertersInstalled;

    private void FixedUpdate()
    {
        if (!convertersInstalled) return;

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
        convertersInstalled = active;
    }

    public bool GetUpgradeInstalled() => convertersInstalled;

    public string GetUpgradeName()
    {
        return "Pressure Converters";
    }

    public void SetUpgradeEnabled(bool enabled)
    {
        // Not needed for this upgrade
    }

    public bool GetUpgradeEnabled() => convertersInstalled;
}
