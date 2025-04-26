using PrototypeSubMod.PowerSystem;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

public class RelayInstallationManager : MonoBehaviour
{
    [SerializeField] private MoonpoolOccupiedHandler occupiedHandler;

    private PrototypePowerSystem powerSystem;
    
    private void Start()
    {
        occupiedHandler.onHasSubChanged.AddListener(OnHasSubChanged);
    }

    private void OnHasSubChanged()
    {
        if (!occupiedHandler.MoonpoolHasSub) return;
        
        powerSystem = occupiedHandler.SubInMoonpool.GetComponentInChildren<PrototypePowerSystem>();
    }
}