using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.MotorHandler;
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
    [SerializeField] private float powerDrainPerSecond;
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
        if (GetUpgradeEnabled())
        {
            subRoot.powerRelay.ConsumeEnergy(powerDrainPerSecond * Time.deltaTime, out _);
        }

        HandleHullBreaches();
    }

    private void HandleHullBreaches()
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

        if (currentTimeBetweenBreaches <= 0)
        {
            if (Random.Range(0f, 1f) < chanceForHullBreach)
            {
                damageManager.CreatePoint();
            }

            currentTimeBetweenBreaches = minTimeBetweenBreaches;
        }
        else
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

    public override void OnSelectedChanged(bool changed)
    {
        if (!changed)
        {
            SetUpgradeEnabled(false);
        }
    }
}
