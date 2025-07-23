using PrototypeSubMod.Upgrades;
using System.Collections.Generic;
using System.Linq;
using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.UI.AbilitySelection;
using PrototypeSubMod.UI.ActivatedAbilities;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoMaintenanceBots : ProtoUpgrade
{
    [SerializeField] private PowerRelay powerRelay;
    [SerializeField] private SelectionMenuManager selectionMenuManager;
    [SerializeField] private ActivatedAbilitiesManager abilitiesManager;
    [SerializeField] private ProtoBotBay[] botBays;
    [SerializeField] private RepairPointManager pointManager;
    [SerializeField] private Transform lavaLarvaeRoot;
    [SerializeField] private float secondsBetweenLarvaClear;
    [SerializeField] private float secondsForOneChargeDrain;

    private int currentBayIndex;
    private float larvaTimer;
    private Queue<CyclopsDamagePoint> queuedPoints = new();

    private void Awake()
    {
        pointManager.onRepairPointCreated += OnDamagePointCreated;
        pointManager.onRepairPointRepaired += OnDamagePointRepaired;
    }

    private void Update()
    {
        if (!upgradeInstalled) return;
        
        if (!upgradeEnabled) return;

        powerRelay.ConsumeEnergy((PrototypePowerSystem.CHARGE_POWER_AMOUNT / secondsForOneChargeDrain) * Time.deltaTime, out _);

        if (larvaTimer < secondsBetweenLarvaClear && lavaLarvaeRoot.childCount > 0)
        {
            larvaTimer += Time.deltaTime;
        }
        else if (lavaLarvaeRoot.childCount > 0)
        {
            larvaTimer = 0;
            var larvaMixin = lavaLarvaeRoot.GetChild(0).GetComponent<LiveMixin>();
            larvaMixin.TakeDamage(1, type: DamageType.Electrical);
        }
        
        if (queuedPoints.Count > 0)
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

    private void AssignBot(CyclopsDamagePoint point)
    {
        botBays[currentBayIndex].QueueBotDeployment(point);
        currentBayIndex++;

        currentBayIndex %= botBays.Length;
    }

    public override bool OnActivated()
    {
        if (upgradeLocked) return false;
        
        SetUpgradeEnabled(!upgradeEnabled);
        return true;
    }

    public override void OnSelectedChanged(bool changed) { }
    public override bool GetCanActivate() => true;
}
