using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.PressureConverters;

internal class ProtoDepthOptimizers : ProtoUpgrade, IPowerModifier
{
    [SerializeField] private CrushDamage crushDamage;
    [SerializeField] private float activationDepth;
    [SerializeField] private int chargeReduction;
    [SerializeField] private float passiveChargeMultiplier;
    [SerializeField] private FMOD_CustomEmitter depthOptimizersActivate;
    [SerializeField] private FMOD_CustomEmitter depthOptimizersDeactivate;

    private float originalCrushDepth;
    
    private void Start()
    {
        originalCrushDepth = crushDamage.crushDepth;
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        base.SetUpgradeEnabled(enabled);

        if (enabled)
        {
            crushDamage.crushDepth = 9000;
            depthOptimizersActivate.Play();
        }
        else
        {
            crushDamage.crushDepth = originalCrushDepth;
            depthOptimizersDeactivate.Play();
        }
    }

    public override bool GetUpgradeEnabled() => upgradeEnabled;

    public override bool OnActivated()
    {
        SetUpgradeEnabled(!upgradeEnabled);
        return true;
    }

    public override void OnSelectedChanged(bool changed) { }

    public void ModifyPowerDrawn(ref float amount)
    {
        if (crushDamage.GetDepth() < activationDepth) return;

        if (amount >= 0) return;

        if (-amount > PrototypePowerSystem.CHARGE_POWER_AMOUNT)
        {
            amount += PrototypePowerSystem.CHARGE_POWER_AMOUNT * chargeReduction;
            return;
        }

        amount *= passiveChargeMultiplier;
    }
}
