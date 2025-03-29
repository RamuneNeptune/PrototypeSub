using Story;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

internal class BuildTerminalScreenManager : MonoBehaviour
{
    [SerializeField] private uGUI_FirstInteractScreen firstInteractScreen;
    [SerializeField] private uGUI_ProtoBuildScreen buildScreen;
    [SerializeField] private TerminalScreen rebuildScreen;
    [SerializeField] private TerminalScreen animatorScreen;
    [SerializeField] private NewUpgradesScreen newUpgradesScreen;
    [SerializeField] private GameObject upgradeScreen;
    [SerializeField] private GameObject emptyScreen;
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;

    private bool isBuilding;
    private int currentStageIndex;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        animatorScreen.gameObject.SetActive(false);
        newUpgradesScreen.gameObject.SetActive(false);
        rebuildScreen.gameObject.SetActive(false);

        if (Plugin.GlobalSaveData.prototypeDestroyed)
        {
            rebuildScreen.gameObject.SetActive(true);

            buildScreen.gameObject.SetActive(false);
            upgradeScreen.gameObject.SetActive(false);
            emptyScreen.gameObject.SetActive(false);
        }
        else if (Plugin.GlobalSaveData.prototypePresent)
        {
            EnableMenusWhenSubInWorld();
        }
        else
        {
            buildScreen.gameObject.SetActive(true);
            rebuildScreen.gameObject.SetActive(false);
            upgradeScreen.gameObject.SetActive(false);
            emptyScreen.gameObject.SetActive(false);
        }

        bool hasInteracted = StoryGoalManager.main.completedGoals.Contains("PlayerFirstPPTInteraction");
        if (!hasInteracted)
        {
            firstInteractScreen.OnStageStarted();
            buildScreen.gameObject.SetActive(false);
            upgradeScreen.gameObject.SetActive(false);
            emptyScreen.gameObject.SetActive(false);
        }
        else
        {
            firstInteractScreen.gameObject.SetActive(false);
        }

        firstInteractScreen.UpdateLightingController();
    }

    public void OnConstructionStarted()
    {
        isBuilding = true;
        rebuildScreen.gameObject.SetActive(false);
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

        if (newUpgradesScreen.HasQueuedUnlocks() || newUpgradesScreen.DownloadActive()) return;

        upgradeScreen.gameObject.SetActive(occupied);
        emptyScreen.gameObject.SetActive(!occupied);
    }

    public void BeginWaitForBuildStage()
    {
        firstInteractScreen.OnStageFinished();
        buildScreen.OnStageStarted();
    }

    public void BeginBuildStage()
    {
        rebuildScreen.gameObject.SetActive(false);
        buildScreen.OnStageFinished();
        animatorScreen.OnStageStarted();
    }

    public void EndBuildStage()
    {
        animatorScreen.OnStageFinished();
        newUpgradesScreen.gameObject.SetActive(false);
        upgradeScreen.gameObject.SetActive(true);
    }

    private void EnableMenusWhenSubInWorld()
    {
        buildScreen.gameObject.SetActive(false);
        rebuildScreen.gameObject.SetActive(false);

        if (newUpgradesScreen.HasQueuedUnlocks())
        {
            newUpgradesScreen.gameObject.SetActive(true);
            upgradeScreen.gameObject.SetActive(false);
            emptyScreen.gameObject.SetActive(false);
        }
        else
        {
            newUpgradesScreen.gameObject.SetActive(false);
            upgradeScreen.gameObject.SetActive(occupiedHandler.MoonpoolHasSub);
            emptyScreen.gameObject.SetActive(!occupiedHandler.MoonpoolHasSub);
        }
    }
}
