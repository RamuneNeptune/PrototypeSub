using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

[RequireComponent(typeof(CanvasGroup))]
internal class UpgradeScreen : MonoBehaviour
{
    [SerializeField] private int maxAllowedUpgrades = 1;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private float startingAlpha;

    private CanvasGroup canvasGroup;
    private float targetAlpha;
    private List<uGUI_ProtoUpgradeIcon> installedUpgrades = new();

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

    public void InstallUpgrade(uGUI_ProtoUpgradeIcon icon)
    {
        installedUpgrades.Add(icon);
    }

    public void UninstallUpgrade(uGUI_ProtoUpgradeIcon icon)
    {
        installedUpgrades.Remove(icon);
    }

    public bool CanInstallNewUpgrade()
    {
        return installedUpgrades.Count < maxAllowedUpgrades;
    }

    public int GetCurrentInstalledUpgradeCount() => installedUpgrades.Count;
}
