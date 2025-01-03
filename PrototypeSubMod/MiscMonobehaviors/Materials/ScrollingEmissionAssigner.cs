using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors.Materials;

internal class ScrollingEmissionAssigner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Transform objectTop;
    [SerializeField] private Transform objectBottom;
    [SerializeField] private bool alwaysUpdateBounds;

    [Header("Animation")]
    [SerializeField] private AnimationCurve noiseOverTime;
    [SerializeField] private float duration;
    [SerializeField] private float timeBetweenPasses;
    [SerializeField] private bool invertDirection;

    private float currentTime;

    private void Start()
    {
        SetupMinMax();
    }

    private void Update()
    {
        if (alwaysUpdateBounds)
        {
            SetupMinMax();
        }

        HandleTargetMoving();
    }

    private void SetupMinMax()
    {
        foreach (var renderer in renderers)
        {
            renderer.material.SetFloat("_ObjMaxY", objectTop.position.y);
            renderer.material.SetFloat("_ObjMinY", objectBottom.position.y);
        }
    }

    private void HandleTargetMoving()
    {
        if (currentTime < duration)
        {
            currentTime += Time.deltaTime;
        }
        else if (currentTime > duration && !IsInvoking(nameof(ResetDuration)))
        {
            Invoke(nameof(ResetDuration), timeBetweenPasses);
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

    private void ResetDuration()
    {
        currentTime = 0;
    }
}
