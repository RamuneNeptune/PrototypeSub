using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.HydroVentilators;

internal class ProtoHydrodynamicVentilators : ProtoUpgrade
{
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

    private bool upgradeInstalled;

    private void FixedUpdate()
    {
        if (!upgradeInstalled) return;

        float depth = crushDamage.GetDepth();

        if (depth < activationDepth)
        {
            motorHandler.SetSpeedMultiplier(1f);
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        motorHandler.SetSpeedMultiplier(multiplier);
    }

    public override void SetUpgradeInstalled(bool installed)
    {
        upgradeInstalled = installed;
    }

    public override bool GetUpgradeInstalled() => upgradeInstalled;

    public override string GetUpgradeName()
    {
        return "Hydro Ventilators";
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        // Not needed for this upgrade
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;
}
