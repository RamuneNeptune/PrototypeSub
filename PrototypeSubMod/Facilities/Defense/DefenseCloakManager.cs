using Story;
using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class DefenseCloakManager : MonoBehaviour
{
    [HideInInspector] public bool isDirty = true;
    [HideInInspector] public float enabledAmount = 1;

    [Header("Shader")]
    public Shader shader;

    [Header("References")]
    public Transform sphere;
    public Transform hexPrism;

    [Header("Fade In")]
    [SerializeField] private float fadeInTime;
    [SerializeField] private AnimationCurve opacityOverFadeIn;

    [Header("Colors")]
    public Color interiorColor;
    public Color distortionColor;
    public Color vignetteColor;
    public Color outsideInColor;
    [Range(0, 1)] public float exteriorCutoutRatio;

    [Header("Distortion")]
    public float effectBoundaryMax;
    public float effectBoundaryMin;
    public float boundaryOffset;
    public float distortionAmplitude;

    [Header("Vignette")]
    public float vignetteIntensity;
    public float vignetteSmoothness;
    public float vignetteOffset;
    public float vignetteFadeInDist;

    [Header("Oscillation")]
    public float oscillationFrequency;
    public float oscillationAmplitude;
    public float oscillationSpeed;
    public int waveCount;
    public float frequencyIncrease = 1.18f;
    public float amplitudeFalloff = 0.82f;

    [Header("Deactivation")]
    [SerializeField] private float scaleTime;
    [SerializeField] private AnimationCurve scaleOverTime;
    [SerializeField] private MultipurposeAlienTerminal deactivationTerminal;
    [SerializeField] private Light[] activatedLights;

    private CloakCutoutApplier cloakApplier;
    private bool deactivated;
    private float currentScaleTime;
    private float originalScale;
    private float currentFadeInTime;

    private float[] lightIntensities;

    private void OnValidate()
    {
        isDirty = true;
    }

    private void Start()
    {
        originalScale = sphere.localScale.x;

        if (Plugin.GlobalSaveData.defenseCloakDisabled)
        {
            sphere.localScale = Vector3.zero;
            deactivationTerminal.ForceInteracted();
        }
        else
        {
            lightIntensities = new float[activatedLights.Length];

            for (int i = 0; i < activatedLights.Length; i++)
            {
                lightIntensities[i] = activatedLights[i].intensity;
                activatedLights[i].intensity = 0;
            }
        }
    }

    public void DeactivateCloak()
    {
        if (Plugin.GlobalSaveData.defenseCloakDisabled) return;

        deactivated = true;
        StoryGoalManager.main.OnGoalComplete("OnDefenseCloakDisabled");
    }

    private void Update()
    {
        if (currentFadeInTime < fadeInTime && !deactivated)
        {
            currentFadeInTime += Time.deltaTime;
        }

        if (!deactivated || currentScaleTime > scaleTime) return;

        if (currentScaleTime < scaleTime)
        {
            currentScaleTime += Time.deltaTime;
            enabledAmount = scaleOverTime.Evaluate(currentScaleTime / scaleTime);
            isDirty = true;

            float intensityMultiplier = currentScaleTime / scaleTime;

            if (currentScaleTime + Time.deltaTime >= scaleTime)
            {
                sphere.localScale = Vector3.zero;
                Plugin.GlobalSaveData.defenseCloakDisabled = true;
                intensityMultiplier = 1;
            }

            for (int i = 0; i < activatedLights.Length; i++)
            {
                activatedLights[i].intensity = lightIntensities[i] * intensityMultiplier;
            }
        }
    }

    private void OnEnable()
    {
        cloakApplier = Camera.main.GetComponent<CloakCutoutApplier>();
        cloakApplier.SetCloakManager(this);

        if (deactivationTerminal != null)
        {
            deactivationTerminal.onTerminalInteracted += DeactivateCloak;
        }
    }

    private void OnDestroy()
    {
        cloakApplier.SetCloakManager(null);

        if (deactivationTerminal != null)
        {
            deactivationTerminal.onTerminalInteracted -= DeactivateCloak;
        }
    }

    public float GetFadeInOpacity()
    {
        return opacityOverFadeIn.Evaluate(currentFadeInTime / fadeInTime);
    }
}
