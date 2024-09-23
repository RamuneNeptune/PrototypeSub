using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoUpgradeIcon : MonoBehaviour
{
    [SerializeField] private DummyTechType techType;
    [SerializeField] private float confirmTime;
    [SerializeField] private float smoothingTime;
    [SerializeField] private float tooltipScreenScale;
    [SerializeField] private float hoveredScaleMultiplier;
    [SerializeField] private float hoveredScaleSnappiness;
    [SerializeField] private Image progressMask;
    [SerializeField] private Sprite backgroundNormalSprite;
    [SerializeField] private Sprite backgroundHoverSprite;
    [SerializeField] private uGUI_ItemIcon itemIcon;
    [SerializeField] private RocketBuilderTooltip tooltip;

    private uGUI_ProtoBuildScreen buildScreen;

    private bool hovered;
    private bool pointerDownLastFrame;
    private bool craftTriggered;
    private float currentConfirmTime;
    private float oldTooltipScale;
    private Vector2 originalSize;
    private RectTransform rectTransform;

    private Atlas.Sprite atlasSpriteBGNormal;
    private Atlas.Sprite atlasSpriteBGHovered;

    private void Start()
    {
        buildScreen = GetComponentInParent<uGUI_ProtoBuildScreen>();

        atlasSpriteBGNormal = new Atlas.Sprite(backgroundNormalSprite);
        atlasSpriteBGHovered = new Atlas.Sprite(backgroundHoverSprite);

        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;

        SetUpgradeTechType(techType.TechType);
    }

    public void SetUpgradeTechType(TechType techType)
    {
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
        if (!buildScreen.IsTooltipActive()) return;

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
    }

    private void FixedUpdate()
    {
        tooltip.gameObject.SetActive(buildScreen.IsTooltipActive());
    }

    private void LateUpdate()
    {
        HandleTooltipActive(GameInput.GetButtonHeld(GameInput.Button.LeftHand));
    }

    private void HandleConfirmCountdown()
    {
        if (pointerDownLastFrame == false)
        {
            craftTriggered = false;
            currentConfirmTime = 0;
        }

        currentConfirmTime = Mathf.MoveTowards(currentConfirmTime, confirmTime, Time.deltaTime * smoothingTime);

        if (currentConfirmTime >= (confirmTime - 0.01f) && !craftTriggered)
        {
            currentConfirmTime = confirmTime;
            craftTriggered = true;

            ProtoUpgradeManager.Instance.SetUpgradeInstalled(techType.TechType, true);
        }
    }

    private void HandleHoverScale()
    {
        Vector2 targetScale = hovered ? originalSize * hoveredScaleMultiplier : originalSize;

        rectTransform.sizeDelta = Vector2.Lerp(rectTransform.sizeDelta, targetScale, Time.deltaTime * hoveredScaleSnappiness);
    }

    private void HandleTooltipActive(bool pointerDown)
    {
        if (hovered && pointerDown)
        {
            uGUI_Tooltip.Clear();
        }
    }

    public void OnPointerEnter(BaseEventData data)
    {
        if (!buildScreen.IsTooltipActive()) return;

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
        uGUI_Tooltip.Clear();
    }

    private void InitializeBGIcon(uGUI_ItemIcon icon)
    {
        var rt = icon.background.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.sizeDelta = new Vector2(0.003f, 0.003f);
    }
}
