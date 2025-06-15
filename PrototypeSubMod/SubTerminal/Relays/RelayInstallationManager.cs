using System;
using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal.Relays;

public class RelayInstallationManager : MonoBehaviour
{
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private SubUpgradeInstallationButton[] installationButtons;

    private PrototypePowerSystem powerSystem;
    private bool hadSubLastFrame;
    
    
    private void Start()
    {
        UpdateIcons();
    }

    private void Update()
    {
        if (occupiedHandler.MoonpoolHasSub != hadSubLastFrame)
        {
            OnHasSubChanged();
        }

        hadSubLastFrame = occupiedHandler.MoonpoolHasSub;
    }

    private void OnHasSubChanged()
    {
        if (!occupiedHandler.MoonpoolHasSub) return;
        
        powerSystem = occupiedHandler.SubInMoonpool.GetComponentInChildren<PrototypePowerSystem>();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        if (powerSystem == null) return;
        
        for (int i = 0; i < installationButtons.Length; i++)
        {
            var button = installationButtons[i];
            button.SetEnabled(i == powerSystem.GetAllowedSourcesCount() - 2);
        }
    }

    public void InstallRelay()
    {
        if (powerSystem == null)
        {
            Plugin.Logger.LogError("Tried to install relay without power system reference");
            return;
        }
        
        powerSystem.SetAllowedSourcesCount(powerSystem.GetAllowedSourcesCount() + 1);
        ErrorMessage.AddError($"Installed new relay. Current count = {powerSystem.GetAllowedSourcesCount()}");
        
        UpdateIcons();
    }
}