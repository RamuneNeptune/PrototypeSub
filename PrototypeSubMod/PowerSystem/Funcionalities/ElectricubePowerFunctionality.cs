using PrototypeSubMod.Monobehaviors;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Teleporter;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class ElectricubePowerFunctionality : PowerSourceFunctionality
{
    private static readonly Color ElectricubeLightColor = new Color(219 / 255f, 191 / 255f, 255 / 255f);
    private static readonly Color ElectricubeTeleporterColor = new Color(0.4089f, 0f, 0.7180f, 0.6282f);

    public static readonly Color TeleportScreenColInner = new Color(0.5638f, 0.4349f, 0.6674f, 0.4970f);
    public static readonly Color TeleportScreenColMiddle = new Color(0.15f, 0.1905f, 1.0000f, 0.3000f);
    public static readonly Color TeleportScreenColOuter = new Color(0.4412f, 0.4285f, 0.7118f, 0.4790f);

    private LightColorHandler colorHandler;
    private ProtoMotorHandler motorHandler;
    private TeleporterFXColorManager colorManager;
    private EmissionColorController emissiveController;

    public override void OnAbilityActivated()
    {
        var root = GetComponentInParent<SubRoot>();
        colorHandler = root.GetComponentInChildren<LightColorHandler>();
        motorHandler = root.GetComponentInChildren<ProtoMotorHandler>();
        colorManager = root.GetComponentInChildren<TeleporterFXColorManager>();
        emissiveController = root.GetComponentInChildren<EmissionColorController>();

        colorHandler.SetTempColor(ElectricubeLightColor);
        motorHandler.AddPowerEfficiencyMultiplier(new ProtoMotorHandler.ValueRegistrar(this, 1.2f));
        colorManager.SetTempColor(ElectricubeTeleporterColor);
        emissiveController.RegisterTempColor(new EmissionColorController.EmissionRegistrarData(this, new Color(1, 0, 1, 1)));

        TeleporterOverride.SetTempTeleporterColor(ElectricubeTeleporterColor);
        ProtoTeleporterManager.Instance.SetPowerMultiplier(0.85f);
        ColorOverrideData overrideData = new ColorOverrideData(true, TeleportScreenColInner, TeleportScreenColMiddle, TeleportScreenColOuter);
        ProtoTeleporterManager.Instance.SetColorOverrideData(overrideData);
    }

    protected override void OnAbilityStopped()
    {
        colorHandler.ResetColor();
        colorManager.ResetColor();
        motorHandler.RemovePowerEfficiencyMultiplier(this);
        emissiveController.RemoveTempColor(this);

        ProtoTeleporterManager.Instance.SetPowerMultiplier(1f);
        ProtoTeleporterManager.Instance.ResetOverrideData();
        TeleporterOverride.ResetTeleporterColor();
    }
}
