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

    private bool selected;

    private void FixedUpdate()
    {
        if (!selected) return;

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

    public override bool GetUpgradeEnabled() => selected;

    private void OnDestroy()
    {
        motorHandler.RemovePowerEfficiencyMultiplier(this);
    }

    public override void OnActivated() { }

    public override void OnSelectedChanged(bool changed)
    {
        selected = changed;
    }
}
