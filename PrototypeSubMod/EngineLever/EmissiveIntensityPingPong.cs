using System.Collections;
using Nautilus.Utility;
using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class EmissiveIntensityPingPong : MonoBehaviour
{
    private static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowStrength = Shader.PropertyToID("_GlowStrength");

    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor = Color.black;
    [SerializeField] private float oscillationSpeed;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private bool activeAtStart;
    
    private bool active;
    
    private void Start()
    {
        SetActive(activeAtStart);
    }

    public void SetActive(bool active)
    {
        this.active = active;
        
        foreach (Renderer renderer in renderers)
        {
            if (!active) renderer.material.SetFloat(GlowStrength, 0);
            
            renderer.material.SetColor(GlowColor, active ? enabledColor : disabledColor);
        }
    }

    public void ToggleActive()
    {
        SetActive(!active);
    }

    private void LateUpdate()
    {
        if (!active) return;

        float glowIntensity =
            UWE.Utils.Unlerp(Mathf.Sin(2 * Mathf.PI * oscillationSpeed * Time.time), -1, 1) * maxIntensity +
            minIntensity;
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetFloat(GlowStrength, glowIntensity);
        }
    }
}
