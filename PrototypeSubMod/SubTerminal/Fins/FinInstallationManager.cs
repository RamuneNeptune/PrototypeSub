using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.SubTerminal.Relays;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal.Fins;

public class FinInstallationManager : MonoBehaviour
{
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private FinInstallationButton[] installationButtons;
    [SerializeField] private Image homeScreen;
    [SerializeField] private Sprite[] homeScreenSprites;
    
    private ProtoFinsManager finsManager;
    
    private void Start()
    {
        occupiedHandler.onHasSubChanged.AddListener(OnHasSubChanged);
        occupiedHandler.CheckBlankSlate();
        UpdateIcons();
    }

    private void OnHasSubChanged()
    {
        finsManager = occupiedHandler.SubInMoonpool.GetComponentInChildren<ProtoFinsManager>();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (finsManager == null) return;
        
        for (int i = 0; i < installationButtons.Length; i++)
        {
            var button = installationButtons[i];
            button.SetCanBuild(i == finsManager.GetInstalledFinCount());
            button.SetConstructed(i < finsManager.GetInstalledFinCount());
        }

        UpdateHomeScreenIcon();
    }

    private void UpdateHomeScreenIcon()
    {
        homeScreen.sprite = homeScreenSprites[finsManager.GetInstalledFinCount()];
    }

    public void InstallNewFin()
    {
        if (!finsManager)
        {
            Plugin.Logger.LogError("Trying to install new fin without reference to fins manager");
            return;
        }

        finsManager.SetInstalledFinCount(finsManager.GetInstalledFinCount() + 1);
        UpdateIcons();
    }
}