using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class EmissiveIntensityPingPong : MonoBehaviour
{
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowStrength = Shader.PropertyToID("_GlowStrength");

    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color disabledColor = Color.black;
    [SerializeField] private float oscillationSpeed;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private bool activeAtStart;

    private bool active;
    private Color[] glowColors;

    private void Start()
    {
        glowColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            var rend = renderers[i];
            glowColors[i] = rend.material.GetColor(GlowColor);
        }
        
        SetActive(activeAtStart);
    }

    public void SetActive(bool active)
    {
        this.active = active;
        
        int index = 0;
        foreach (Renderer renderer in renderers)
        {
            if (!active) renderer.material.SetFloat(GlowStrength, 0);
            
            renderer.material.SetColor(GlowColor, active ? glowColors[index] : disabledColor);
            index++;
        }
    }

    public void ToggleActive()
    {
        SetActive(!active);
    }

    private void Update()
    {
        if (!active) return;

        float glowIntensity = UWE.Utils.Unlerp(Mathf.Sin(2 * Mathf.PI * oscillationSpeed * Time.time), -1, 1) * maxIntensity + minIntensity;
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetFloat(GlowStrength, glowIntensity);
        }
    }
}
