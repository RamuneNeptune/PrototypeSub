using PrototypeSubMod.Interfaces;
using PrototypeSubMod.MotorHandler;
using UnityEngine;

namespace PrototypeSubMod.Overclock;

internal class ProtoOverclockModule : MonoBehaviour, IProtoUpgrade
{
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private CyclopsExternalDamageManager damageManager;
    [SerializeField] private ProtoMotorHandler motorHandler;
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
        powerRelay.ConsumeEnergy(powerDrainPerSecond * Time.deltaTime, out _);

        HandleHullBreaches();
        
    }

    private void HandleHullBreaches()
    {
        if (upgradeEnabled && currentHullBreachTime < hullBreachMinActiveTime)
        {
            currentHullBreachTime += Time.deltaTime;
        }
        else if (!upgradeEnabled)
        {
            currentHullBreachTime -= Time.deltaTime;
        }

        if (currentHullBreachTime < hullBreachMinActiveTime)
        {
            return;
        }

        if (currentHullBreachTime <= 0)
        {
            if (Random.Range(0, 1) < chanceForHullBreach)
            {
                damageManager.CreatePoint();
            }

            currentHullBreachTime = minTimeBetweenBreaches;
        }
        else
        {
            currentHullBreachTime -= Time.deltaTime;
        }
    }

    public void SetUpgradeInstalled(bool installed)
    {
        upgradeInstalled = installed;
    }

    public void SetUpgradeEnabled(bool enabled)
    {
        upgradeEnabled = enabled;
    }

    public string GetUpgradeName()
    {
        return "Overclock Module";
    }

    public bool GetUpgradeEnabled() => upgradeEnabled;
    public bool GetUpgradeInstalled() => upgradeInstalled;
}
