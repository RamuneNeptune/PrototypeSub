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
            motorHandler.SetPowerMultiplier(1f);
            return;
        }

        float normalizedDepth = (depth - activationDepth) / (maxDepth - activationDepth);
        float multiplier = powerMultiplierCurve.Evaluate(normalizedDepth);

        motorHandler.SetPowerMultiplier(multiplier);
    }

    public override bool GetUpgradeEnabled() => upgradeInstalled;
}
