using UnityEngine;

namespace PrototypeSubMod.Teleporter;

public class TeleporterMapManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup surfaceGroup;
    [SerializeField] private CanvasGroup depthsGroup;
    [SerializeField] private float transitionDuration;

    private bool targetingSurface;
    private float currentTransitionDuration;

    private void Start()
    {
        surfaceGroup.alpha = 1;
        depthsGroup.alpha = 0;
        targetingSurface = true;
    }

    private void Update()
    {
        if (currentTransitionDuration < transitionDuration)
        {
            currentTransitionDuration += Time.deltaTime;
        }
        
        float normalizedProgress = currentTransitionDuration / transitionDuration;
        float surfaceAlpha = targetingSurface ? normalizedProgress : 1 - normalizedProgress;
        float depthsAlpha =  1 - surfaceAlpha;
        surfaceGroup.alpha = surfaceAlpha;
        surfaceGroup.blocksRaycasts = targetingSurface;
        depthsGroup.alpha = depthsAlpha;
        depthsGroup.blocksRaycasts = !targetingSurface;
    }

    public void SetTargetDepth(bool atSurface)
    {
        targetingSurface = atSurface;
        currentTransitionDuration = 0;
    }

    public void ToggleDepth()
    {
        SetTargetDepth(!targetingSurface);
    }
}