using PrototypeSubMod.Prefabs;
using System.Collections;
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

    private void Awake()
    {
        tooltip.rocketTechType = Prototype_Craftable.SubInfo.TechType;

        UWE.CoroutineHost.StartCoroutine(UpdateTooltipActive());
    }

    public void OnConstructPressed()
    {
        onBuildStarted.Invoke();
    }

    public IEnumerator UpdateTooltipActive()
    {
        while(true)
        {
            bool inTrigger = distanceTracker.distanceToPlayer <= 4;

            tooltipsActive = inTrigger;
            tooltip.gameObject.SetActive(tooltipsActive);

            yield return new WaitForSeconds(0.25f);
        }
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
