using PrototypeSubMod.Prefabs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoBuildScreen : MonoBehaviour
{
    [SerializeField] private UnityEvent onBuildStarted;
    [SerializeField] private RocketBuilderTooltip tooltip;
    [SerializeField] private TextMeshProUGUI constructButtonText;
    [SerializeField] private PlayerDistanceTracker distanceTracker;
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private GameObject buildScreen;
    [SerializeField] private uGUI_BuildAnimScreen animationScreen;
    [SerializeField] private GameObject occupiedScreen;

    private bool isBuilding;

    private void Start()
    {
        constructButtonText.text = Language.main.Get("ConstructButton");
        tooltip.rocketTechType = Prototype_Craftable.SubInfo.TechType;
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
        onBuildStarted.Invoke();
    }

    public void OnConstructionStarted(float duration)
    {
        buildScreen.SetActive(false);
        animationScreen.gameObject.SetActive(true);
        animationScreen.StartAnimation(duration);

        isBuilding = true;
    }

    //Called by VFX Constructing in Update via SendMessage()
    public void OnConstructionDone()
    {
        buildScreen.SetActive(false);
        animationScreen.gameObject.SetActive(false);
        occupiedScreen.SetActive(true);

        isBuilding = false;
    }

    public void OnOccupiedChanged()
    {
        if (isBuilding) return;

        var occupied = occupiedHandler.MoonpoolHasSub;

        buildScreen.SetActive(!occupied);
        occupiedScreen.SetActive(occupied);
    }

    private void UpdateTooltipActive()
    {
        bool flag = distanceTracker.distanceToPlayer < 5f;
        tooltip.gameObject.SetActive(flag);
    }
}
