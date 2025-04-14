using PrototypeSubMod.Upgrades;
using System.Collections.Generic;
using System.Linq;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.UI.AbilitySelection;
using PrototypeSubMod.UI.ActivatedAbilities;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoMaintenanceBots : ProtoUpgrade, IOnTakeDamage
{
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private SelectionMenuManager selectionMenuManager;
    [SerializeField] private ActivatedAbilitiesManager abilitiesManager;
    [SerializeField] private ProtoBotBay[] botBays;
    [SerializeField] private RepairPointManager pointManager;
    [SerializeField] private float minTimeSinceDamage;
    [SerializeField] private float secondsForOneChargeDrain;

    private int currentBayIndex;
    private float timeLastDamage = float.MinValue;
    private Queue<CyclopsDamagePoint> queuedPoints = new();

    private void Start()
    {
        pointManager.onRepairPointCreated += OnDamagePointCreated;
        pointManager.onRepairPointRepaired += OnDamagePointRepaired;
    }

    private void Update()
    {
        if (!upgradeInstalled) return;
        
        if (!upgradeEnabled) return;

        powerRelay.ConsumeEnergy((PrototypePowerSystem.CHARGE_POWER_AMOUNT / secondsForOneChargeDrain) * Time.deltaTime, out _);
        
        if (timeLastDamage + minTimeSinceDamage < Time.time && queuedPoints.Count > 0)
        {
            for (int i = 0; i < queuedPoints.Count; i++)
            {
                AssignBot(queuedPoints.Dequeue());
            }
        }
    }

    private void OnDamagePointCreated(CyclopsDamagePoint point)
    {
        queuedPoints.Enqueue(point);
    }

    private void OnDamagePointRepaired(CyclopsDamagePoint point)
    {
        queuedPoints = new Queue<CyclopsDamagePoint>(queuedPoints.Where(p => p != point));
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (upgradeEnabled)
        {
            SetUpgradeEnabled(false);
            var icon = selectionMenuManager.GetAbilityIcons().FirstOrDefault(i => i == (IAbilityIcon)this);
            abilitiesManager.OnAbilitySelectedChanged(icon);
        }
        
        if (damageInfo.type != DamageType.Normal && damageInfo.type != DamageType.Electrical)
        {
            return;
        }

        timeLastDamage = Time.time;
    }

    private void AssignBot(CyclopsDamagePoint point)
    {
        botBays[currentBayIndex].QueueBotDeployment(point);
        currentBayIndex++;

        currentBayIndex %= botBays.Length;
    }

    public override void SetUpgradeInstalled(bool installed)
    {
        base.SetUpgradeInstalled(installed);

        if (upgradeInstalled)
        {
            timeLastDamage = float.MinValue;
        }
    }

    public override void OnActivated()
    {
        SetUpgradeEnabled(!upgradeEnabled);
    }

    public override void OnSelectedChanged(bool changed) { }

    public override bool GetCanActivate()
    {
        return timeLastDamage + minTimeSinceDamage < Time.time;
    }
}
