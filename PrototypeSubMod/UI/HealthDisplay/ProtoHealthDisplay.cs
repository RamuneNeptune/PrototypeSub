using PrototypeSubMod.RepairBots;
using UnityEngine;

namespace PrototypeSubMod.UI.HealthDisplay;

public class ProtoHealthDisplay : MonoBehaviour, IOnTakeDamage
{
    [SerializeField] private LiveMixin liveMixin;
    [SerializeField] private RepairPointManager repairPointManager;

    private void Start()
    {
        repairPointManager.onRepairPointRepaired += _ => UpdateHealth();
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        float normalizedHealth = liveMixin.health / liveMixin.maxHealth;
        int currentSegmentCount = Mathf.CeilToInt(normalizedHealth * 10);
        float latestSegmentAmount = (normalizedHealth % 0.1f) * 10;
        Plugin.Logger.LogInfo($"Current Segment Count: {currentSegmentCount} |  Latest Segment Amount: {latestSegmentAmount}");
    }
}