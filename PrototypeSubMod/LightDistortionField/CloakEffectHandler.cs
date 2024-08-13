using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class CloakEffectHandler : MonoBehaviour
{
    public static CloakEffectHandler Instance { get; private set; }

    [Header("Shader Parameters")]
    public Shader shader;
    public Transform ovoid;
    public Color color;
    public Color distortionColor;
    public float falloffMultiplier;
    public float distortionBoundaryMin;
    public float distortionBoundaryMax;
    public float distortionBoundaryOffset;
    public float distortionAmplitude;

    [Header("Animation")]
    public float scaleSpeed;

    private float targetScaleMultiplier;
    private Vector3 originalScale;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        originalScale = ovoid.localScale;
    }

    private void Update()
    {
        ovoid.localScale = Vector3.MoveTowards(ovoid.localScale, originalScale * targetScaleMultiplier, Time.deltaTime * scaleSpeed);
    }
}
