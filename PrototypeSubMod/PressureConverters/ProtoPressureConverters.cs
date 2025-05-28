using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

internal class ProtoPressureConverters : ProtoUpgrade
{
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private float maxDepth;
    [SerializeField] private AnimationCurve powerMultiplierCurve;

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
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        motorHandler.AddPowerEfficiencyMultiplier(new ProtoMotorHandler.ValueRegistrar(this, multiplier));
    }

    private void OnDestroy()
    {
        motorHandler.RemovePowerEfficiencyMultiplier(this);
    }

    public override bool GetUpgradeEnabled() => upgradeEnabled;

    public override bool OnActivated() => false;
    public override void OnSelectedChanged(bool changed) { }
}
