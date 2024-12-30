using Story;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_FirstInteractScreen : TerminalScreen
{
    [SerializeField] private BuildTerminalScreenManager screenManager;
    [SerializeField] private GameObject normalObjects;
    [SerializeField] private GameObject loadingObjects;
    [SerializeField] private Image loadingBar;
    [SerializeField] private AnimationCurve progressOverTime;
    [SerializeField] private float loadingTime;

    private float currentLoadingTime;
    private bool loadingStarted;

    private void Start()
    {
        loadingBar.fillAmount = 0;
    }

    public override void OnStageStarted()
    {
        gameObject.SetActive(true);
    }

    public override void OnStageFinished()
    {
        gameObject.SetActive(false);
        StoryGoalManager.main.OnGoalComplete("PlayerFirstPPTInteraction");
    }

    public void OnInteract()
    {
        loadingStarted = true;
        normalObjects.SetActive(false);
        loadingObjects.SetActive(true);
    }

    private void Update()
    {
        if (!loadingStarted) return;

        if (currentLoadingTime < loadingTime)
        {
            currentLoadingTime += Time.deltaTime;
            loadingBar.fillAmount = progressOverTime.Evaluate(currentLoadingTime / loadingTime);
        }
        else
        {
            screenManager.BeginWaitForBuildStage();
            loadingStarted = false;
        }
    }
}
