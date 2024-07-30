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
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    public void OnConstructPressed()
    {
        OnBuildStarted?.Invoke();
    }

    private void UpdateTooltipActive()
    {

    }
}
