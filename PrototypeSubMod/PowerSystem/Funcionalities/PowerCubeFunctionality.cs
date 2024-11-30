using PrototypeSubMod.IonBarrier;
using PrototypeSubMod.Monobehaviors;
using PrototypeSubMod.MotorHandler;
using UnityEngine;

namespace PrototypeSubMod.PowerSystem.Funcionalities;

internal class PowerCubeFunctionality : PowerSourceFunctionality
{
    private const float MAX_SPEED_MULTIPLIER = 1.1f;
    private const float BARRIER_DAMAGE_MULTIPLIER = 0.85f;
    private readonly Color barrierMainCol = new Color(0.91f, 0.05f, 0.15f, 0.35f);
    private readonly Color barrierSolidCol = new Color(0.55f, 0.25f, 0.25f, 0.098f);

    private ProtoMotorHandler motorHandler;
    private ProtoIonBarrier ionBarrier;
    private ShieldEffectManager shieldEffectManager;
    private EmissionColorController emissiveController;

    public override void OnAbilityActivated()
    {
        var subRoot = GetComponentInParent<SubRoot>();
        motorHandler = subRoot.GetComponentInChildren<ProtoMotorHandler>();
        ionBarrier = subRoot.GetComponentInChildren<ProtoIonBarrier>();
        shieldEffectManager = subRoot.GetComponentInChildren<ShieldEffectManager>();
        emissiveController = subRoot.GetComponentInChildren<EmissionColorController>();

        motorHandler.AddSpeedMultiplier(new ProtoMotorHandler.ValueRegistrar(this, MAX_SPEED_MULTIPLIER));
        ionBarrier.SetDamageReductionMultiplier(BARRIER_DAMAGE_MULTIPLIER);
        shieldEffectManager.SetTempColor(barrierMainCol, barrierSolidCol);
        emissiveController.RegisterTempColor(new EmissionColorController.EmissionRegistrarData(this, new Color(1, 0, 0, 1)));
    }

    protected override void OnAbilityStopped()
    {
        motorHandler.RemoveSpeedMultiplier(this);
        ionBarrier.SetDamageReductionMultiplier(1f);
        shieldEffectManager.ClearTempColor();
        emissiveController.RemoveTempColor(this);
    }
}
