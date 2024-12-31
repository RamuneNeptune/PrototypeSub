using Story;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_FirstInteractScreen : TerminalScreen
{
    [SerializeField] private BuildTerminalScreenManager screenManager;
    [SerializeField] private LightingController lightingController;
    [SerializeField] private GameObject normalObjects;
    [SerializeField] private GameObject loadingObjects;
    [SerializeField] private Image loadingBar;
    [SerializeField] private Image logoImage;
    [SerializeField] private AnimationCurve progressOverTime;
    [SerializeField] private float loadingTime;

    private float currentLoadingTime;
    private bool loadingStarted;

    private void Start()
    {
        loadingBar.fillAmount = 0;
        logoImage.material.SetFloat("_LoadProgress", 0);

        if (StoryGoalManager.main.completedGoals.Contains("PlayerFirstPPTInteraction"))
        {
            lightingController.LerpToState(1, 1);
        }
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
        lightingController.LerpToState(1, loadingTime * 2);
    }

    private void Update()
    {
        if (!loadingStarted) return;

        if (currentLoadingTime < loadingTime)
        {
            currentLoadingTime += Time.deltaTime;
            float fillAmount = progressOverTime.Evaluate(currentLoadingTime / loadingTime);
            loadingBar.fillAmount = fillAmount;
            logoImage.material.SetFloat("_LoadProgress", fillAmount);
        }
        else
        {
            screenManager.BeginWaitForBuildStage();
            loadingStarted = false;
        }
    }
}
