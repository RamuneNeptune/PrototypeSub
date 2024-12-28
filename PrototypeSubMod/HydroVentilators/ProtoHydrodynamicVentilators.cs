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
    [SerializeField] private AnimationCurve speedMultiplierCurve;

    private bool speedReset;

    private void FixedUpdate()
    {
        if (!upgradeInstalled) return;

        float depth = crushDamage.GetDepth();

        if (depth < activationDepth)
        {
            motorHandler.RemovePowerEfficiencyMultiplier(this);
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = speedMultiplierCurve.Evaluate(normalizedDepth);

        motorHandler.AddSpeedMultiplier(new ProtoMotorHandler.ValueRegistrar(this, multiplier));
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;

    private void OnDestroy()
    {
        motorHandler.RemoveSpeedMultiplier(this);
    }
}
