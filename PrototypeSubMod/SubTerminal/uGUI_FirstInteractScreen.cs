using Story;
using System.Collections;
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

    [Header("Background Glitch")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite glitchSprite;
    [SerializeField] private float[] glitchTimePoints;
    [SerializeField] private float glitchDuration;

    private float currentLoadingTime;
    private float previousLoadingTime;
    private bool loadingStarted;

    private void Start()
    {
        loadingBar.fillAmount = 0;
        logoImage.material.SetFloat("_LoadProgress", 0);

        if (StoryGoalManager.main.completedGoals.Contains("PlayerFirstPPTInteraction"))
        {
            lightingController.LerpToState(1, 1);
        }

        normalObjects.SetActive(true);
        loadingObjects.SetActive(false);
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
            float normalizedProgress = currentLoadingTime / loadingTime;
            float fillAmount = progressOverTime.Evaluate(normalizedProgress);
            loadingBar.fillAmount = fillAmount;
            logoImage.material.SetFloat("_LoadProgress", fillAmount);
        }
        else
        {
            screenManager.BeginWaitForBuildStage();
            loadingStarted = false;
        }

        HandleGlitchPoints();

        previousLoadingTime = currentLoadingTime;
    }

    private void HandleGlitchPoints()
    {
        float normalizedProgress = currentLoadingTime / loadingTime;
        float prevNormalizedProgress = previousLoadingTime / loadingTime;

        for (int i = 0; i < glitchTimePoints.Length; i++)
        {
            float timePoint = glitchTimePoints[i];
            if (prevNormalizedProgress < timePoint && normalizedProgress > timePoint)
            {
                backgroundImage.sprite = glitchSprite;
                UWE.CoroutineHost.StartCoroutine(ResetBGSprite());
            }
        }
    }

    private IEnumerator ResetBGSprite()
    {
        yield return new WaitForSeconds(glitchDuration);

        backgroundImage.sprite = normalSprite;
    }
}
