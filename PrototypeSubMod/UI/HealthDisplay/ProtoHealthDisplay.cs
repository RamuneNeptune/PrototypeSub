using System;
using PrototypeSubMod.RepairBots;
using PrototypeSubMod.UI.ProceduralArcGenerator;
using SubLibrary.UI;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace PrototypeSubMod.UI.HealthDisplay;

public class ProtoHealthDisplay : MonoBehaviour, IOnTakeDamage, IUIElement
{
    [SerializeField] private LiveMixin liveMixin;
    [SerializeField] private RepairPointManager repairPointManager;
    [SerializeField] private Image[] healthNotches;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color lowHealthColor;
    
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
        for (int i = 0; i < healthNotches.Length; i++)
        {
            healthNotches[i].gameObject.SetActive(i < currentSegmentCount);
            var color = (i + 1) == currentSegmentCount && lowHealth ? lowHealthColor : normalColor;
            healthNotches[i].color = color;
        }
    }

    public void UpdateUI() { }

    public void OnSubDestroyed()
    {
        foreach (var notch in healthNotches)
        {
            notch.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        liveMixin.onHealDamage.RemoveHandler(gameObject);
        liveMixin.onHealTempDamage.RemoveHandler(gameObject);
    }
}