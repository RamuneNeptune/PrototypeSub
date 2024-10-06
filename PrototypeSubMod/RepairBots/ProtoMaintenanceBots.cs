using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.RepairBots;

internal class ProtoMaintenanceBots : ProtoUpgrade
{
    [SerializeField] private ProtoBotBay[] botBays;
    [SerializeField] private RepairPointManager pointManager;

    private int currentBayIndex;

    private void Start()
    {
        pointManager.onRepairPointCreated += OnDamagePointCreated;
        pointManager.onRepairPointRepaired += OnDamagePointRemoved;
    }

    private void OnDamagePointCreated(CyclopsDamagePoint point)
    {
        if (!upgradeInstalled) return;

        botBays[currentBayIndex].QueueBotDeployment(point);
    }

    private void OnDamagePointRemoved(CyclopsDamagePoint damagePoint)
    {
        if (!upgradeInstalled) return;
    }
}
