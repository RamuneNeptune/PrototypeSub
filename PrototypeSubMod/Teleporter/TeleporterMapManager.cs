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
    }

    private void Update()
    {
        if (currentTransitionDuration < transitionDuration)
        {
            currentTransitionDuration += Time.deltaTime;
        }
        
        float normalizedProgress = currentTransitionDuration / transitionDuration;
        float surfaceAlpha = targetingSurface ? normalizedProgress : 1 - normalizedProgress;
        float depthsAlpha =  targetingSurface ? 1 - normalizedProgress : normalizedProgress;
        surfaceGroup.alpha = surfaceAlpha;
        depthsGroup.alpha = depthsAlpha;
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