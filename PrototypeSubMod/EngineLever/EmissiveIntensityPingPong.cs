using UnityEngine;

namespace PrototypeSubMod.EngineLever;

internal class EmissiveIntensityPingPong : MonoBehaviour
{
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private float osciallationSpeed;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;

    private bool active;

    private void Start()
    {
        SetActive(false);
    }

    public void SetActive(bool active)
    {
        this.active = active;

        if (!active)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetFloat("_GlowStrength", 0);
            }
        }
    }

    public void ToggleActive()
    {
        SetActive(!active);
    }

    private void Update()
    {
        if (!active) return;

        float glowIntensity = UWE.Utils.Unlerp(Mathf.Sin(2 * Mathf.PI * osciallationSpeed * Time.time), -1, 1) * maxIntensity + minIntensity;
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetFloat("_GlowStrength", glowIntensity);
        }
    }
}
