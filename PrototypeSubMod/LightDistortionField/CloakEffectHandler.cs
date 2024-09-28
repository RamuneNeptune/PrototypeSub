using PrototypeSubMod.IonGenerator;
using PrototypeSubMod.Upgrades;
using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class CloakEffectHandler : ProtoUpgrade
{
    public static CloakEffectHandler Instance { get; private set; }

    [Header("Shader Parameters")]
    public Shader shader;
    public Transform ovoid;

    [Header("Colors")]
    public Color color;
    public Color distortionColor;
    public Color interiorColor;
    public Color vignetteColor;

    [Header("Distortion")]
    public float falloffMultiplier;
    public float distortionBoundaryMin;
    public float distortionBoundaryMax;
    public float distortionBoundaryOffset;
    public float distortionAmplitude;

    [Header("Vignette")]
    public float vignetteIntensity;
    public float vignetteSmoothness;
    public float vignetteOffset;
    public float vignetteFadeInDist;

    [Header("Animation")]
    public float scaleSpeed;

    [Header("Sound Values")]
    public float soundMultiplier;

    [Header("Miscellaneous")]
    public ProtoIonGenerator ionGenerator;

    private float TargetScaleMultiplier
    {
        get
        {
            return GetAllowedToCloak() ? 1 : 0;
        }
    }

    private Vector3 originalScale;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        originalScale = ovoid.localScale;
        ovoid.localScale = originalScale * TargetScaleMultiplier;
    }

    private void Update()
    {
        ovoid.localScale = Vector3.MoveTowards(ovoid.localScale, originalScale * TargetScaleMultiplier, Time.deltaTime * scaleSpeed);
    }

    public bool IsInsideOvoid(Vector3 point)
    {
        if (!upgradeEnabled) return false;

        Vector3 localPoint = point - ovoid.position;

        localPoint = Quaternion.Inverse(ovoid.rotation) * localPoint;

        Vector3 normalizedPoint = Divide(localPoint, ovoid.localScale);

        return normalizedPoint.sqrMagnitude < 1;
    }

    private Vector3 Divide(Vector3 lhs, Vector3 rhs)
    {
        return new Vector3(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
    }

    public bool GetAllowedToCloak()
    {
        return !ionGenerator.GetUpgradeInstalled() && upgradeEnabled && upgradeInstalled;
    }

    public float GetTargetScale()
    {
        return TargetScaleMultiplier;
    }
}
