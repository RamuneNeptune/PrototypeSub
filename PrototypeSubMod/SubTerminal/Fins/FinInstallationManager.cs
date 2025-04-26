using PrototypeSubMod.PowerSystem;
using PrototypeSubMod.SubTerminal.Relays;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal.Fins;

public class FinInstallationManager : MonoBehaviour
{
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private SubUpgradeInstallationButton[] installationButtons;
    
    private void Start()
    {
        occupiedHandler.onHasSubChanged.AddListener(OnHasSubChanged);
        UpdateIcons();
    }

    private void OnHasSubChanged()
    {
        if (!occupiedHandler.MoonpoolHasSub) return;
        
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        /*
        for (int i = 0; i < installationButtons.Length; i++)
        {
            var button = installationButtons[i];
            button.SetEnabled(i == powerSystem.GetAllowedSourcesCount() - 2);
        }
        */
    }
}