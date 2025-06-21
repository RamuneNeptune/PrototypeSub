using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.PressureConverters;
using PrototypeSubMod.SaveData;
using PrototypeSubMod.Upgrades;
using SubLibrary.SaveData;
using UnityEngine;

namespace PrototypeSubMod.VariablePowerStreams;

internal class ProtoVariablePowerStreams : ProtoUpgrade, IPowerModifier
{
    [SerializeField] private float passivePowerEfficiency = 0.85f;
    
    public override bool OnActivated() => false;
    public override void OnSelectedChanged(bool changed) { }

    public void ModifyPowerDrawn(ref float amount)
    {
        if (amount > PrototypePowerSystem.CHARGE_POWER_AMOUNT) return;

        if (!upgradeInstalled) return;

        amount *= passivePowerEfficiency;
    }
}
