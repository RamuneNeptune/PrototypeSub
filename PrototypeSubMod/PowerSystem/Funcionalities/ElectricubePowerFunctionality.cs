using PrototypeSubMod.Monobehaviors;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Teleporter;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class ElectricubePowerFunctionality : PowerSourceFunctionality
{
    private static readonly Color ElectricubeLightColor = new Color(219 / 255f, 191 / 255f, 255 / 255f);

    private LightColorHandler colorHandler;
    private ProtoMotorHandler motorHandler;

    public override void OnAbilityActivated()
    {
        var root = GetComponentInParent<SubRoot>();
        colorHandler = root.GetComponentInChildren<LightColorHandler>();
        motorHandler = root.GetComponentInChildren<ProtoMotorHandler>();

        colorHandler.SetTempColor(ElectricubeLightColor);
        motorHandler.SetPowerEfficiencyMultiplier(1.2f);
        ProtoTeleporterManager.Instance.SetPowerMultiplier(0.85f);
        TeleporterOverride.SetTempTeleporterColor(ElectricubeLightColor);
    }

    protected override void OnAbilityStopped()
    {
        colorHandler.ResetColor();
        motorHandler.SetPowerEfficiencyMultiplier(1f);
        ProtoTeleporterManager.Instance.SetPowerMultiplier(1f);
        TeleporterOverride.ResetTeleporterColor();
    }
}
