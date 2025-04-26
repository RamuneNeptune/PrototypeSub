using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal.Relays;

public class RelayInstallationManager : MonoBehaviour
{
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;
    [SerializeField] private RelayInstallationButton[] installationButtons;

    private PrototypePowerSystem powerSystem;
    
    private void Start()
    {
        occupiedHandler.onHasSubChanged.AddListener(OnHasSubChanged);
    }

    private void OnHasSubChanged()
    {
        if (!occupiedHandler.MoonpoolHasSub) return;
        
        powerSystem = occupiedHandler.SubInMoonpool.GetComponentInChildren<PrototypePowerSystem>();
        UpdateIcons();
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < installationButtons.Length; i++)
        {
            var button = installationButtons[i];
            button.SetColor(i >= powerSystem.GetAllowedSourcesCount() - 2 ? 0.8f : 0.5f);
        }
    }
}