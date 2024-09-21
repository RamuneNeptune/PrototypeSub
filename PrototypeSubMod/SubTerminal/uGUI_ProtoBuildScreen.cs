using PrototypeSubMod.Prefabs;
using UnityEngine;
using UnityEngine.Events;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoBuildScreen : MonoBehaviour
{
    [SerializeField] private UnityEvent onBuildStarted;

    [SerializeField] private ProtoBuildTerminal buildTerminal;
    [SerializeField] private RocketBuilderTooltip tooltip;
    [SerializeField] private PlayerDistanceTracker distanceTracker;
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private GameObject buildScreen;
    [SerializeField] private uGUI_BuildAnimScreen animationScreen;
    [SerializeField] private GameObject upgradeScreen;
    [SerializeField] private GameObject emptyScreen;
    [SerializeField] private Animator armAnimator;

    private bool isBuilding;

    private void Start()
    {
        tooltip.rocketTechType = Prototype_Craftable.SubInfo.TechType;
        if (buildTerminal.HasBuiltProtoSub)
        {
            upgradeScreen.SetActive(occupiedHandler.MoonpoolHasSub);
            emptyScreen.SetActive(!occupiedHandler.MoonpoolHasSub);
            buildScreen.SetActive(false);
        }
        else
        {
            buildScreen.SetActive(true);
        }
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

    private void UpdateTooltipActive()
    {
        bool flag = distanceTracker.distanceToPlayer < 5f;
        tooltip.gameObject.SetActive(flag);

        bool flag2 = distanceTracker.distanceToPlayer < 7f;
        armAnimator.SetBool("ScreenActivated", flag2);
    }
}
