using PrototypeSubMod.MotorHandler;
using System;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class IonPowerCellFunctionality : PowerSourceFunctionality
{
    private const float ENGINE_EFFICIENCY_MULTIPLIER = 1.15f;

    private ProtoMotorHandler motorHandler;

    public override void OnAbilityActivated()
    {
        var root = GetComponentInParent<SubRoot>();
        motorHandler = root.GetComponentInChildren<ProtoMotorHandler>();

        motorHandler.SetPowerEfficiencyMultiplier(ENGINE_EFFICIENCY_MULTIPLIER);
    }

    protected override void OnAbilityStopped()
    {
        motorHandler.SetPowerEfficiencyMultiplier(1f);
    }
}
