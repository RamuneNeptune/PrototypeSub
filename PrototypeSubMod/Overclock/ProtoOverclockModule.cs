using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Upgrades;
using UnityEngine;
using UnityEngine.Serialization;

namespace PrototypeSubMod.Overclock;

internal class ProtoOverclockModule : ProtoUpgrade
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsExternalDamageManager damageManager;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private VoiceNotification enabledVoiceline;
    [SerializeField] private VoiceNotification invalidOperationNotification;
    [SerializeField] private float speedBaseBonus;
    [SerializeField] private float turningTorqueMultiplier;
    [SerializeField] private float secondsToDrainCharge;
    [SerializeField, Range(0, 1)] private float chanceForHullBreach;
    [SerializeField] private float hullBreachMinActiveTime;
    [SerializeField] private float minTimeBetweenBreaches;

    private float currentHullBreachTime;
    private float currentTimeBetweenBreaches;

    private void Update()
    {
        if (!upgradeInstalled || ionGenerator.GetUpgradeEnabled())
        {
            motorHandler.RemoveSpeedBonus(this);
            motorHandler.RemoveTurningTorqueMultiplier(this);
            return;
        }

        bool couldConsume = false;
        if (upgradeEnabled)
        {
            couldConsume = subRoot.powerRelay.ConsumeEnergy(
                PrototypePowerSystem.CHARGE_POWER_AMOUNT / secondsToDrainCharge * Time.deltaTime, out _);
        }
        
        HandleHullBreaches(couldConsume);

        if (!upgradeEnabled)
        {
            motorHandler.RemoveSpeedBonus(this);
            motorHandler.RemoveTurningTorqueMultiplier(this);
            return;
        }
        
        motorHandler.AddSpeedBonus(new ProtoMotorHandler.ValueRegistrar(this, speedBaseBonus));
        motorHandler.AddTurningTorqueMultiplier(new  ProtoMotorHandler.ValueRegistrar(this, turningTorqueMultiplier));
    }

    private void HandleHullBreaches(bool couldConsume)
    {
        if (GetUpgradeEnabled() && currentHullBreachTime < hullBreachMinActiveTime)
        {
            currentHullBreachTime += Time.deltaTime;
        }
        else if (!GetUpgradeEnabled() && currentHullBreachTime > 0)
        {
            currentHullBreachTime -= Time.deltaTime;
        }

        if (currentHullBreachTime < hullBreachMinActiveTime)
        {
            return;
        }

        if (currentTimeBetweenBreaches <= 0 && couldConsume)
        {
            if (Random.Range(0f, 1f) < chanceForHullBreach)
            {
                damageManager.CreatePoint();
            }

            currentTimeBetweenBreaches = minTimeBetweenBreaches;
        }
        else if (couldConsume)
        {
            currentTimeBetweenBreaches -= Time.deltaTime;
        }
    }

    public override bool GetUpgradeEnabled()
    {
        return upgradeEnabled && !ionGenerator.GetUpgradeEnabled();
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        base.SetUpgradeEnabled(enabled);

        if (upgradeEnabled)
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(enabledVoiceline);
        }
    }

    public override void OnActivated()
    {
        if (ionGenerator.GetUpgradeEnabled())
        {
            subRoot.voiceNotificationManager.PlayVoiceNotification(invalidOperationNotification);
            return;
        }
        
        SetUpgradeEnabled(!upgradeEnabled);
    }

    public override void OnSelectedChanged(bool changed) { }
}
