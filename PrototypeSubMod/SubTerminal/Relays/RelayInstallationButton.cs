using PrototypeSubMod.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal.Relays;

public class RelayInstallationButton : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private RelayInstallationManager installationManager;
    [SerializeField] private DummyTechType relayUpgradeTechType;
    [SerializeField] private RocketBuilderTooltip tooltip;

    private bool tooltipActive;
    private bool buttonEnabled;
    
    private void Start()
    {
        tooltip.rocketTechType = relayUpgradeTechType.TechType;
        InvokeRepeating(nameof(UpdateTooltipActive), 0, 1);
    }
    
    public void SetEnabled(bool enabled)
    {
        buttonEnabled = enabled;
        float fillCol = enabled ? 0.8f : 0.5f;
        
        foreach (var image in images)
        {
            image.color = new Color(fillCol, fillCol,fillCol);
        }

        if (!enabled)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    private void UpdateTooltipActive()
    {
        if (!buttonEnabled) return;
        
        tooltipActive = (Player.main.transform.position - transform.position).sqrMagnitude < 9f;
        tooltip.gameObject.SetActive(tooltipActive);
    }

    public void OnClick()
    {
        if (!CrafterLogic.ConsumeResources(relayUpgradeTechType.TechType)) return;

        installationManager.InstallRelay();
    }
}