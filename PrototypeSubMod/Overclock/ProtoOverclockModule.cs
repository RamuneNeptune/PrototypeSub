using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.MotorHandler;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.Overclock;

internal class ProtoOverclockModule : ProtoUpgrade
{
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private CyclopsExternalDamageManager damageManager;
    [SerializeField] private ProtoMotorHandler motorHandler;
    [SerializeField] private ProtoIonGenerator ionGenerator;
    [SerializeField] private float speedPercentBonus;
    [SerializeField] private float powerDrainPerSecond;
    [SerializeField, Range(0, 1)] private float chanceForHullBreach;
    [SerializeField] private float hullBreachMinActiveTime;
    [SerializeField] private float minTimeBetweenBreaches;

    private bool upgradeInstalled;
    private bool upgradeEnabled;
    private float currentHullBreachTime;
    private float currentTimeBetweenBreaches;

    private void Update()
    {
        if (!upgradeInstalled)
        {
            motorHandler.SetSpeedMultiplierBonus(0);
            return;
        }

        float speedBonus = upgradeEnabled ? speedPercentBonus / 100f : 0;
        motorHandler.SetSpeedMultiplierBonus(speedBonus);
        if (upgradeEnabled)
        {
            powerRelay.ConsumeEnergy(powerDrainPerSecond * Time.deltaTime, out _);
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

    public override void SetUpgradeInstalled(bool installed)
    {
        upgradeInstalled = installed;
    }

    public override void SetUpgradeEnabled(bool enabled)
    {
        upgradeEnabled = enabled;
    }

    public override string GetUpgradeName()
    {
        return "Overclock Module";
    }

    public override bool GetUpgradeEnabled()
    {
        return upgradeEnabled && !ionGenerator.GetUpgradeEnabled();
    }
    public override bool GetUpgradeInstalled() => upgradeInstalled;
}
