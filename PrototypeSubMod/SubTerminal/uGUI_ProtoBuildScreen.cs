using PrototypeSubMod.Prefabs;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoBuildScreen : MonoBehaviour
{
    [SerializeField] private UnityEvent onBuildStarted;

    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private RocketBuilderTooltip tooltip;
    [SerializeField] private ProtoPlayerDistanceTracker distanceTracker;
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private GameObject buildScreen;
    [SerializeField] private uGUI_BuildAnimScreen animationScreen;
    [SerializeField] private GameObject upgradeScreen;
    [SerializeField] private GameObject emptyScreen;

    private bool isBuilding;
    private bool tooltipsActive;

    private void Start()
    {
        tooltip.rocketTechType = Prototype_Craftable.SubInfo.TechType;

        if (Plugin.GlobalSaveData.prototypePresent)
        {
            upgradeScreen.SetActive(occupiedHandler.MoonpoolHasSub);
            emptyScreen.SetActive(!occupiedHandler.MoonpoolHasSub);
            buildScreen.SetActive(false);
        }
        else
        {
            buildScreen.SetActive(true);
            upgradeScreen.SetActive(false);
            emptyScreen.SetActive(false);
        }

        distanceTracker.OnPlayerTriggerChanged += UpdateTooltipActive;
    }


    public void OnConstructPressed()
    {
        onBuildStarted.Invoke();
    }

    public void OnConstructionPreWarm(float duration)
    {
        buildScreen.SetActive(false);
        animationScreen.gameObject.SetActive(true);
        animationScreen.StartPreWarm(duration);
    }

    public void OnConstructionStarted(float duration)
    {
        animationScreen.StartAnimation(duration);

        isBuilding = true;
    }

    public void OnConstructionAsyncStarted()
    {
        tooltip.gameObject.SetActive(false);
    }

    // Called by VFX Constructing in Update via SendMessage()
    public void OnConstructionDone()
    {
        buildScreen.SetActive(false);
        animationScreen.gameObject.SetActive(false);
        upgradeScreen.SetActive(true);

        isBuilding = false;
    }

    public void OnOccupiedChanged()
    {
        if (isBuilding) return;

        var occupied = occupiedHandler.MoonpoolHasSub;

        upgradeScreen.SetActive(occupied);
        emptyScreen.SetActive(!occupied);
    }

    public void UpdateTooltipActive(bool inTrigger)
    {
        tooltipsActive = inTrigger;
        tooltip.gameObject.SetActive(tooltipsActive);
    }

    public bool IsTooltipActive()
    {
        return tooltipsActive;
    }

    public MoonpoolOccupiedHandler GetMoonpoolHandler() => occupiedHandler;
}
