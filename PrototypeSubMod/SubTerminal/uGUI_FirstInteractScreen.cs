using Story;
using System;
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

    [Header("Exposition")]
    [SerializeField] private VoiceNotificationManager manager;
    [SerializeField] private ExpositionData[] datas;

    [SerializeField, HideInInspector] public VoiceNotification[] notifications;
    [SerializeField, HideInInspector] public float[] durations;

    private float currentLoadingTime;
    private float previousLoadingTime;
    private bool loadingStarted;

    private void OnValidate()
    {
        notifications = new VoiceNotification[datas.Length];
        durations = new float[datas.Length];

        for (int i = 0; i < datas.Length; i++)
        {
            notifications[i] = datas[i].voiceline;
            durations[i] = datas[i].duration;
        }
    }

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
            loadingStarted = false;
            StartCoroutine(OrionExposition());
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

    private IEnumerator OrionExposition()
    {
        for (int i = 0; i < notifications.Length; i++)
        {
            manager.PlayVoiceNotification(notifications[i], false, true);

            yield return new WaitForSeconds(durations[i]);
        }

        screenManager.BeginWaitForBuildStage();
    }

    [Serializable]
    private class ExpositionData
    {
        public VoiceNotification voiceline;
        public float duration;
    }
}
