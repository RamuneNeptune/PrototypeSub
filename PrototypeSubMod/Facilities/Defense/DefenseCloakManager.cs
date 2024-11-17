using UnityEngine;

namespace PrototypeSubMod.Facilities.Defense;

internal class DefenseCloakManager : MonoBehaviour
{
    [HideInInspector] public bool isDirty;

    [Header("Shader")]
    public Shader shader;

    [Header("References")]
    public Transform referencesParent;
    public Transform sphere;
    public Transform hexPrism;

    [Header("Colors")]
    public Color interiorColor;
    public Color distortionColor;
    public Color vignetteColor;

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

    [Header("Activation")]
    [SerializeField] private float scaleTime;
    [SerializeField] private AnimationCurve scaleOverTime;

    private CloakCutoutApplier cloakApplier;
    private bool deactivated;
    private float currentScaleTime;

    private void OnValidate()
    {
        isDirty = true;
    }

    public void DeactivateCloak()
    {
        if (Plugin.GlobalSaveData.defenseCloakDisabled) return;

        deactivated = true;
        Plugin.GlobalSaveData.defenseCloakDisabled = true;
    }

    private void Update()
    {
        if (!deactivated || currentScaleTime > scaleTime) return;

        if (currentScaleTime < scaleTime)
        {
            currentScaleTime += Time.deltaTime;
            referencesParent.localScale = Vector3.one * scaleOverTime.Evaluate(currentScaleTime / scaleTime);
        }
    }

    private void OnEnable()
    {
        cloakApplier = Camera.main.GetComponent<CloakCutoutApplier>();
        cloakApplier.SetCloakManager(this);

        if (Plugin.GlobalSaveData.defenseCloakDisabled)
        {
            referencesParent.localScale = Vector3.zero;
        }
    }

    private void OnDestroy()
    {
        cloakApplier.SetCloakManager(null);
    }
}
