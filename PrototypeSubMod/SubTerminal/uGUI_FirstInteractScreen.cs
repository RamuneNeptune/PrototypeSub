using Story;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_FirstInteractScreen : TerminalScreen
{
    [SerializeField] private BuildTerminalScreenManager screenManager;
    [SerializeField] private FMOD_CustomEmitter activationSFX;
    [SerializeField] private LightingController lightingController;
    [SerializeField] private GameObject normalObjects;
    [SerializeField] private GameObject loadingObjects;
    [SerializeField] private Image loadingBar;
    [SerializeField] private AnimationCurve progressOverTime;
    [SerializeField] private float loadingTime;
    [SerializeField] private string devResumedPDAKey;
    [SerializeField] private float voicelineWaitTime;

    [Header("Background Glitch")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite glitchSprite;
    [SerializeField] private float[] glitchTimePoints;
    [SerializeField] private float glitchDuration;
    
    private float currentLoadingTime;
    private float previousLoadingTime;
    private bool loadingStarted;
    private bool voicelinesStarted;

    private void Awake()
    {
        UpdateLightingController();
    }
    
    private void Start()
    {
        loadingBar.fillAmount = 0;
        
        normalObjects.SetActive(true);
        loadingObjects.SetActive(false);
    }

    public void UpdateLightingController()
    {
        if (StoryGoalManager.main.completedGoals.Contains("PlayerFirstPPTInteraction"))
        {
            lightingController.SnapToState(1);
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
        activationSFX.Play();
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

            if (currentLoadingTime >= loadingTime * 0.75f && !voicelinesStarted)
            {
                voicelinesStarted = true;
                StartCoroutine(OrionExposition());
            }
        }
        else
        {
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

    private IEnumerator OrionExposition()
    {
        PDALog.Add(devResumedPDAKey);

        yield return new WaitForSeconds(voicelineWaitTime);

        screenManager.BeginWaitForBuildStage();
    }

    [Serializable]
    private class ExpositionData
    {
        public VoiceNotification voiceline;
        public float duration;
    }
}
