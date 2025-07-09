using PrototypeSubMod.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal.Relays;

public class RelayInstallationButton : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private DummyTechType relayUpgradeTechType;
    [SerializeField] private RocketBuilderTooltip tooltip;
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private Sprite constructedSprite;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite hoveredSprite;

    [SerializeField] private FMODAsset onEnterSfx;
    [SerializeField] private FMODAsset onClickSfx;
    [SerializeField] private float volume;
    
    private bool tooltipActive;
    private bool canBuild;
    private bool hovered;
    
    private void Start()
    {
        tooltip.rocketTechType = relayUpgradeTechType.TechType;
        InvokeRepeating(nameof(UpdateTooltipActive), 0, 1);
    }
    
    public void SetCanBuild(bool canBuild)
    {
        this.canBuild = canBuild;
        
        if (!canBuild)
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public void SetConstructed(bool constructed)
    {
        if (!constructed) return;
        
        image.sprite = constructedSprite;
    }

    public void OnPointerEnter(BaseEventData eventData)
    {
        hovered = true;
        if (onEnterSfx) FMODUWE.PlayOneShot(onEnterSfx, transform.position, volume);
    }
    
    public void OnPointerExit(BaseEventData eventData)
    {
        hovered = false;
    }
    
    private void Update()
    {
        if (Time.deltaTime == 0 || !canBuild) return;
        
        image.sprite = hovered ?  hoveredSprite : normalSprite;
    }

    private void UpdateTooltipActive()
    {
        if (!canBuild) return;
        
        tooltipActive = (Player.main.transform.position - transform.position).sqrMagnitude < 9f;
        tooltip.gameObject.SetActive(tooltipActive);
    }

    public void OnClick()
    {
        if (!CrafterLogic.ConsumeResources(relayUpgradeTechType.TechType)) return;

        onClick?.Invoke();
        if (onClickSfx) FMODUWE.PlayOneShot(onClickSfx, transform.position, volume);
    }
}