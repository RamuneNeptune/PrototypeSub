using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.Overclock;

internal class ProtoOverclockModule : ProtoUpgrade
{
    [SerializeField] private SubRoot subRoot;
    [SerializeField] private CyclopsExternalDamageManager damageManager;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private VoiceNotification enabledVoiceline;
    [SerializeField] private float speedPercentBonus;
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
            motorHandler.RemoveSpeedMultiplierBonus(this);
            return;
        }

        float speedBonus = upgradeEnabled ? speedPercentBonus / 100f : 0;
        motorHandler.AddSpeedMultiplierBonus(new ProtoMotorHandler.ValueRegistrar(this, speedBonus));
        bool couldConsume = false;
        if (GetUpgradeEnabled())
        {
            couldConsume = subRoot.powerRelay.ConsumeEnergy(PrototypePowerSystem.CHARGE_POWER_AMOUNT / secondsToDrainCharge * Time.deltaTime, out _);
        }

        HandleHullBreaches(couldConsume);
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
        SetUpgradeEnabled(!upgradeEnabled);
    }

    public override void OnSelectedChanged(bool changed) { }
}
