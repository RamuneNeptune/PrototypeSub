using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class BuildTerminalScreenManager : MonoBehaviour
{
    [SerializeField] private TerminalScreen buildScreen;
    [SerializeField] private TerminalScreen animatorScreen;
    [SerializeField] private GameObject upgradeScreen;
    [SerializeField] private GameObject emptyScreen;
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;

    private bool isBuilding;
    private int currentStageIndex;

    private void Start()
    {
        animatorScreen.gameObject.SetActive(false);

        if (Plugin.GlobalSaveData.prototypePresent)
        {
            upgradeScreen.gameObject.SetActive(occupiedHandler.MoonpoolHasSub);
            emptyScreen.gameObject.SetActive(!occupiedHandler.MoonpoolHasSub);
            buildScreen.gameObject.SetActive(false);
        }
        else
        {
            buildScreen.gameObject.SetActive(true);
            upgradeScreen.gameObject.SetActive(false);
            emptyScreen.gameObject.SetActive(false);
        }
    }

    public void OnConstructionStarted()
    {
        isBuilding = true;
    }

    // Called by VFX Constructing in Update via SendMessage()
    public void OnConstructionDone()
    {
        isBuilding = false;
    }

    // Called via UnityEvent
    public void OnOccupiedChanged()
    {
        if (isBuilding) return;

        var occupied = occupiedHandler.MoonpoolHasSub;

        upgradeScreen.gameObject.SetActive(occupied);
        emptyScreen.gameObject.SetActive(!occupied);
    }

    public void BeginBuildStage()
    {
        buildScreen.OnStageFinished();
        animatorScreen.OnStageStarted();
    }

    public void EndBuildStage()
    {
        animatorScreen.OnStageFinished();
        upgradeScreen.gameObject.SetActive(true);
    }
}
