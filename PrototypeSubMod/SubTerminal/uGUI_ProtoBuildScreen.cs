using PrototypeSubMod.Prefabs;
using System;
using TMPro;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoBuildScreen : MonoBehaviour
{
    public event Action OnBuildStarted;

    [SerializeField] private RocketBuilderTooltip tooltip;
    [SerializeField] private TextMeshProUGUI constructButtonText;
    [SerializeField] private PlayerDistanceTracker distanceTracker;

    private void Start()
    {
        constructButtonText.text = Language.main.Get("ConstructButton");
        tooltip.rocketTechType = Prototype_Craftable.SubInfo.TechType;

        throw new Exception("This exception makes me go yes.");
    }

    private void OnEnable()
    {
        CancelInvoke(nameof(UpdateTooltipActive));
        InvokeRepeating(nameof(UpdateTooltipActive), 0f, 0.5f);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(UpdateTooltipActive));
    }

    public void OnConstructPressed()
    {
        OnBuildStarted?.Invoke();
    }

    private void UpdateTooltipActive()
    {
        bool flag = distanceTracker.distanceToPlayer < 5f;
        tooltip.gameObject.SetActive(flag);
    }
}
