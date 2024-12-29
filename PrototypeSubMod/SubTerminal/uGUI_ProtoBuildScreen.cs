using PrototypeSubMod.Prefabs;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoBuildScreen : TerminalScreen
{
    [SerializeField] private UnityEvent onBuildStarted;

    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private RocketBuilderTooltip tooltip;
    [SerializeField] private PlayerDistanceTracker distanceTracker;
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;

    private bool tooltipsActive;

    private void Start()
    {
        tooltip.rocketTechType = Prototype_Craftable.SubInfo.TechType;

        InvokeRepeating(nameof(UpdateTooltipActive), 0, 0.25f);
    }

    public void OnConstructPressed()
    {
        onBuildStarted.Invoke();
    }

    public void UpdateTooltipActive()
    {
        bool inTrigger = distanceTracker.distanceToPlayer <= 2;

        tooltipsActive = inTrigger;
        tooltip.gameObject.SetActive(tooltipsActive);
    }

    public bool IsTooltipActive()
    {
        return tooltipsActive;
    }

    public MoonpoolOccupiedHandler GetMoonpoolHandler() => occupiedHandler;

    public override void OnStageStarted()
    {
        gameObject.SetActive(true);
    }

    public override void OnStageFinished()
    {
        tooltip.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
