using UnityEngine;

namespace PrototypeSubMod.LightDistortionField;

internal class CloakEffectHandler : MonoBehaviour
{
    public static CloakEffectHandler Instance { get; private set; }

    [SerializeField] private Shader shader;
    [SerializeField] private Transform sphere;
    [SerializeField] private float falloffMultiplier;
    [SerializeField] private Color color;
    [SerializeField] private float scaleSpeed;

    private float targetScale;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        float scale = Mathf.Lerp(sphere.localScale.x, targetScale, Time.deltaTime * scaleSpeed);
        sphere.localScale = Vector3.one * scale;
    }

    public Shader GetShader() => shader;
    public Transform GetSphere() => sphere;
    public float GetFalloffMultiplier() => falloffMultiplier;
    public Color GetEffectColor() => color;
}
