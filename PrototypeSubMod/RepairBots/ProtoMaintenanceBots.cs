using PrototypeSubMod.Upgrades;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoMaintenanceBots : ProtoUpgrade, IOnTakeDamage
{
    [SerializeField] private ProtoBotBay[] botBays;
    [SerializeField] private RepairPointManager pointManager;
    [SerializeField] private float minTimeSinceDamage;

    private int currentBayIndex;
    private float timeSinceDamage = float.MaxValue;
    private Queue<CyclopsDamagePoint> queuedPoints = new();

    private void Start()
    {
        pointManager.onRepairPointCreated += OnDamagePointCreated;
        pointManager.onRepairPointRepaired += OnDamagePointRepaired;
    }

    private void Update()
    {
        if (!upgradeInstalled) return;

        if (timeSinceDamage < minTimeSinceDamage)
        {
            timeSinceDamage += Time.deltaTime;
        }
        else if (queuedPoints.Count > 0)
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

        if (!upgradeInstalled) return;
    }

    private void OnDamagePointRepaired(CyclopsDamagePoint point)
    {
        queuedPoints = new Queue<CyclopsDamagePoint>(queuedPoints.Where(p => p != point));
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        if (damageInfo.type != DamageType.Normal && damageInfo.type != DamageType.Electrical)
        {
            return;
        }

        timeSinceDamage = 0;
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
            timeSinceDamage = float.MaxValue;
        }
    }
}
