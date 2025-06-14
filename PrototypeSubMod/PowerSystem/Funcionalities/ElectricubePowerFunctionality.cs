using PrototypeSubMod.MiscMonobehaviors;
using PrototypeSubMod.MiscMonobehaviors.Emission;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Teleporter;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class ElectricubePowerFunctionality : PowerSourceFunctionality
{
    private static readonly Color ElectricubeLightColor = new Color(219 / 255f, 191 / 255f, 255 / 255f);

    private LightColorHandler colorHandler;
    private ProtoMotorHandler motorHandler;
    private TeleporterFXColorManager colorManager;
    private EmissionColorController emissiveController;
    private ProtoTeleporterManager teleporterManager;

    public override void OnAbilityActivated()
    {
        var root = GetComponentInParent<SubRoot>();
        colorHandler = root.GetComponentInChildren<LightColorHandler>();
        motorHandler = root.GetComponentInChildren<ProtoMotorHandler>();
        colorManager = root.GetComponentInChildren<TeleporterFXColorManager>();
        emissiveController = root.GetComponentInChildren<EmissionColorController>();
        teleporterManager = root.GetComponentInChildren<ProtoTeleporterManager>();

        colorHandler.SetTempColor(ElectricubeLightColor);
        motorHandler.AddPowerEfficiencyMultiplier(new ProtoMotorHandler.ValueRegistrar(this, 1.2f));
        colorManager.AddTempColor(this, new TeleporterFXColorManager.TempColor(ElectricubeLightColor));
        emissiveController.RegisterTempColor(new EmissionColorController.EmissionRegistrarData(this, new Color(1, 0, 1, 1)));
    }

    protected override void OnAbilityStopped()
    {
        colorHandler.ResetColor();
        colorManager.RemoveTempColor(this);
        motorHandler.RemovePowerEfficiencyMultiplier(this);
        emissiveController.RemoveTempColor(this);
    }
}
