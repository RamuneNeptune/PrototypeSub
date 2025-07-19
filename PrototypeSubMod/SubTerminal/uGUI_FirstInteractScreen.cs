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
    
    private float currentLoadingTime;
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
