using System;
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
    
    private void Start()
    {
        liveMixin.onHealDamage.AddHandler(gameObject, (_) => UpdateHealth());
        liveMixin.onHealTempDamage.AddHandler(gameObject, (_) => UpdateHealth());
        
        UpdateHealth();
    }

    private void OnEnable()
    {
        UpdateHealth();
    }

    public void OnTakeDamage(DamageInfo damageInfo)
    {
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        const int incrementCount = 10;
        float normalizedHealth = liveMixin.health / liveMixin.maxHealth;
        int currentSegmentCount = Mathf.CeilToInt(normalizedHealth * incrementCount);
        float lastSegmentAmount = normalizedHealth % 0.1f * incrementCount;

        bool lowHealth = lastSegmentAmount < 0.5f;
        if (lowHealthArcGenerator.gameObject.activeSelf != lowHealth)
        {
            lowHealthArcGenerator.gameObject.SetActive(lowHealth);
        }
        
        int maskIndex = maskAngles.Length - currentSegmentCount - 1;
        
        normalArcGenerator.SetTargetAngles(360, maskAngles[Mathf.Min(maskAngles.Length - 1, maskIndex + (lowHealth ? 1 : 0))]);
        lowHealthArcGenerator.SetTargetAngles(maskAngles[Mathf.Min(maskAngles.Length - 1, maskIndex + 1)], maskAngles[maskIndex]);

        normalArcGenerator.GenerateMesh();
        lowHealthArcGenerator.GenerateMesh();
    }

    public void UpdateUI() { }

    public void OnSubDestroyed()
    {
        lowHealthArcGenerator.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        liveMixin.onHealDamage.RemoveHandler(gameObject);
        liveMixin.onHealTempDamage.RemoveHandler(gameObject);
    }
}