using System;
using UnityEngine;

namespace PrototypeSubMod.MiscMonobehaviors;

public class CanvasFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float transitionSpeed = 1;

    private float targetAlpha;

    public void SetVisible()
    {
        targetAlpha = 1;
    }

    public void SetInvisible()
    {
        targetAlpha = 0;
    }

    private void Update()
    {
        if (Mathf.Approximately(canvasGroup.alpha, targetAlpha)) return;

        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime * transitionSpeed);
    }
}