using PrototypeSubMod.RepairBots;
using PrototypeSubMod.UI.ProceduralArcGenerator;
using SubLibrary.UI;
using UnityEngine;

namespace PrototypeSubMod.UI.HealthDisplay;

public class ProtoHealthDisplay : MonoBehaviour, IOnTakeDamage, IUIElement
{
    [SerializeField] private LiveMixin liveMixin;
    [SerializeField] private RepairPointManager repairPointManager;
    [SerializeField] private CircularMeshGenerator normalArcGenerator;
    [SerializeField] private CircularMeshGenerator lowHealthArcGenerator;
    [SerializeField] private int[] maskAngles;

    private int segmentCountLastCheck;
    private bool subDead;
    
    private void Start()
    {
        repairPointManager.onRepairPointRepaired += _ => UpdateHealth();
        UpdateHealth();
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        UpdateHealth();
    }

    private void UpdateHealth()
    {
        if (subDead) return;
        
        int incrementCount = 10;
        float normalizedHealth = liveMixin.health / liveMixin.maxHealth;
        int currentSegmentCount = Mathf.CeilToInt(normalizedHealth * incrementCount);
        float lastSegmentAmount = normalizedHealth % 0.1f * incrementCount;

        if (lowHealthArcGenerator.gameObject.activeSelf != lastSegmentAmount < 0.25f)
        {
            lowHealthArcGenerator.gameObject.SetActive(lastSegmentAmount < 0.25f);
        }

        if (currentSegmentCount == segmentCountLastCheck) return;
        
        int maskIndex = maskAngles.Length - currentSegmentCount - 1;
        normalArcGenerator.SetTargetAngles(360, maskAngles[maskIndex]);
        lowHealthArcGenerator.SetTargetAngles(maskAngles[maskIndex], maskAngles[Mathf.Max(0, maskIndex - 1)]);

        normalArcGenerator.GenerateMesh();
        lowHealthArcGenerator.GenerateMesh();
        
        segmentCountLastCheck =  currentSegmentCount;
    }

    public void UpdateUI() { }

    public void OnSubDestroyed()
    {
        lowHealthArcGenerator.gameObject.SetActive(false);
        subDead = true;
    }
}