using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

[RequireComponent(typeof(RocketBuilderTooltip))]
internal class uGUI_ProtoUpgradeIcon : MonoBehaviour
{
    public Action<TechType> onCraftPressed;

    [SerializeField] private float confirmTime;
    [SerializeField] private float smoothingTime;
    [SerializeField] private Image progressMask;
    [SerializeField] private Sprite backgroundNormalSprite;
    [SerializeField] private Sprite backgroundHoverSprite;
    [SerializeField] private uGUI_ItemIcon itemIcon;

    private RocketBuilderTooltip tooltip;
    private TechType techType;

    private bool hovered;
    private bool pointerDownLastFrame;
    private bool craftTriggered;
    private float currentConfirmTime;
    private float lastConfirmTime;

    private Atlas.Sprite atlasSpriteBGNormal;
    private Atlas.Sprite atlasSpriteBGHovered;

    private void Start()
    {
        tooltip = GetComponent<RocketBuilderTooltip>();
    }

    public void SetUpgradeTechType(TechType techType)
    {
        this.techType = techType;
        tooltip.rocketTechType = techType;
        itemIcon.SetForegroundSprite(SpriteManager.Get(techType));
        itemIcon.SetForegroundSize(new Vector2(0.15f, 0.15f));

        atlasSpriteBGNormal = new Atlas.Sprite(backgroundNormalSprite);
        atlasSpriteBGHovered = new Atlas.Sprite(backgroundHoverSprite);

        itemIcon.SetBackgroundSprite(atlasSpriteBGNormal);
        itemIcon.SetBackgroundSize(new Vector2(0.2f, 0.2f));
    }

    private void Update()
    {
        bool pointerDown = GameInput.GetButtonHeld(GameInput.Button.LeftHand);

        if(hovered && pointerDown)
        {
            if(pointerDownLastFrame == false)
            {
                craftTriggered = false;
                currentConfirmTime = 0;
            }

            float velocity = currentConfirmTime - lastConfirmTime;
            currentConfirmTime = Mathf.SmoothDamp(currentConfirmTime, confirmTime, ref velocity, smoothingTime);
            
            if(currentConfirmTime >= (confirmTime - 0.01f) && !craftTriggered)
            {
                currentConfirmTime = confirmTime;
                onCraftPressed?.Invoke(techType);
                craftTriggered = true;
            }
        }
        else
        {
            currentConfirmTime = confirmTime;
        }

        progressMask.fillAmount = currentConfirmTime / confirmTime;
        pointerDownLastFrame = pointerDown;
        lastConfirmTime = currentConfirmTime;
    }

    public void OnPointerEnter(BaseEventData data)
    {
        hovered = true;
        itemIcon.SetBackgroundSprite(atlasSpriteBGHovered);
        itemIcon.SetBackgroundSize(new Vector2(0.2f, 0.2f));
    }

    public void OnPointerExit(BaseEventData data)
    {
        hovered = false;
        itemIcon.SetBackgroundSprite(atlasSpriteBGNormal);
        itemIcon.SetBackgroundSize(new Vector2(0.2f, 0.2f));
    }
}
