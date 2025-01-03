using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ScrollingEmissionAssigner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Transform objectTop;
    [SerializeField] private Transform objectBottom;

    [Header("Animation")]
    [SerializeField] private AnimationCurve noiseOverTime;
    [SerializeField] private float duration;
    [SerializeField] private float timeBetweenPasses;
    [SerializeField] private bool invertDirection;

    private float currentTime;

    private void Start()
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_ObjMaxY", objectTop.position.y);
            renderer.material.SetFloat("_ObjMinY", objectBottom.position.y);
        }
    }

    private void Update()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
        }
        else if (currentTime > duration)
        {
            currentTime = 0;
        }

        float normaliedValue = currentTime / duration;
        if (invertDirection)
        {
            normaliedValue = 1 - normaliedValue;
        }

        foreach (var rend in renderers)
        {
            rend.material.SetFloat("_UVTarget", normaliedValue);
            rend.material.SetFloat("_NoiseMultiplier", noiseOverTime.Evaluate(normaliedValue));
        }
    }
}
