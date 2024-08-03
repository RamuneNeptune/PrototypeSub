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
    [SerializeField] private float tooltipScreenScale;
    [SerializeField] private float hoveredScaleMultiplier;
    [SerializeField] private float hoveredScaleSnappiness;
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
    private float oldTooltipScale;
    private Vector2 originalSize;
    private RectTransform rectTransform;

    private Atlas.Sprite atlasSpriteBGNormal;
    private Atlas.Sprite atlasSpriteBGHovered;

    private void Start()
    {
        tooltip = GetComponent<RocketBuilderTooltip>();

        atlasSpriteBGNormal = new Atlas.Sprite(backgroundNormalSprite);
        atlasSpriteBGHovered = new Atlas.Sprite(backgroundHoverSprite);

        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;
    }

    public void SetUpgradeTechType(TechType techType)
    {
        this.techType = techType;
        tooltip.rocketTechType = techType;

        itemIcon.SetForegroundSprite(SpriteManager.Get(techType));
        var rt = itemIcon.foreground.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.sizeDelta = new Vector2(0.001f, 0.001f);

        itemIcon.SetBackgroundSprite(atlasSpriteBGNormal);
        InitializeBGIcon(itemIcon);
    }

    private void Update()
    {
        bool pointerDown = GameInput.GetButtonHeld(GameInput.Button.LeftHand);

        if (hovered && pointerDown)
        {
            HandleConfirmCountdown();
        }
        else
        {
            currentConfirmTime = confirmTime;
        }

        HandleHoverScale();

        progressMask.fillAmount = currentConfirmTime / confirmTime;
        pointerDownLastFrame = pointerDown;
        lastConfirmTime = currentConfirmTime;
    }

    private void HandleConfirmCountdown()
    {
        if (pointerDownLastFrame == false)
        {
            craftTriggered = false;
            currentConfirmTime = 0;
        }

        float velocity = currentConfirmTime - lastConfirmTime;
        currentConfirmTime = Mathf.SmoothDamp(currentConfirmTime, confirmTime, ref velocity, smoothingTime);

        if (currentConfirmTime >= (confirmTime - 0.01f) && !craftTriggered)
        {
            currentConfirmTime = confirmTime;
            onCraftPressed?.Invoke(techType);
            craftTriggered = true;
        }
    }

    private void HandleHoverScale()
    {
        Vector2 targetScale = hovered ? rectTransform.sizeDelta * hoveredScaleMultiplier : rectTransform.sizeDelta;

        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, targetScale, Time.deltaTime * hoveredScaleSnappiness);
    }

    public void OnPointerEnter(BaseEventData data)
    {
        hovered = true;
        itemIcon.SetBackgroundSprite(atlasSpriteBGHovered);
        InitializeBGIcon(itemIcon);

        oldTooltipScale = uGUI_Tooltip.main.scaleFactor;
        uGUI_Tooltip.main.scaleFactor = tooltipScreenScale;
    }

    public void OnPointerExit(BaseEventData data)
    {
        hovered = false;
        itemIcon.SetBackgroundSprite(atlasSpriteBGNormal);
        InitializeBGIcon(itemIcon);

        uGUI_Tooltip.main.scaleFactor = oldTooltipScale;
    }

    private void InitializeBGIcon(uGUI_ItemIcon icon)
    {
        var rt = icon.background.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.sizeDelta = new Vector2(0.003f, 0.003f);
    }
}
