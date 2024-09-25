using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

[RequireComponent(typeof(CanvasGroup))]
internal class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private int maxAllowedUpgrades;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private float startingAlpha;

    private CanvasGroup canvasGroup;
    private float targetAlpha;
    private int currentInstalledUpgrades;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        targetAlpha = startingAlpha;
        canvasGroup.alpha = startingAlpha;
        canvasGroup.blocksRaycasts = startingAlpha > 0;
    }

    private void Update()
    {
        canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, Time.deltaTime * transitionSpeed);
    }

    public void SetTargetAlpha(float alpha)
    {
        targetAlpha = alpha;
    }

    public void SetInteractable(bool interactable)
    {
        canvasGroup.blocksRaycasts = interactable;
    }

    public void IncrementUpgradeCount(int delta)
    {
        currentInstalledUpgrades += delta;
    }

    public bool CanInstallNewUpgrade()
    {
        return currentInstalledUpgrades < maxAllowedUpgrades;
    }

    public int GetCurrentInstalledUpgradeCount() => currentInstalledUpgrades;
}
