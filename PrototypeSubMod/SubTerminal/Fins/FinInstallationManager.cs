using PrototypeSubMod.MiscMonobehaviors.SubSystems;
using PrototypeSubMod.SubTerminal.Relays;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal.Fins;

public class FinInstallationManager : MonoBehaviour
{
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private SubUpgradeInstallationButton[] installationButtons;

    private ProtoFinsManager finsManager;
    
    private void Start()
    {
        occupiedHandler.onHasSubChanged.AddListener(OnHasSubChanged);
        UpdateIcons();
    }

    private void OnHasSubChanged()
    {
        if (!occupiedHandler.MoonpoolHasSub) return;
        
        finsManager = occupiedHandler.SubInMoonpool.GetComponentInChildren<ProtoFinsManager>();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (finsManager == null) return;
        
        for (int i = 0; i < installationButtons.Length; i++)
        {
            var button = installationButtons[i];
            button.SetEnabled(i == finsManager.GetInstalledFinCount());
        }
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