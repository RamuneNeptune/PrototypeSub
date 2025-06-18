using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UWE;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoUpgradeIcon : MonoBehaviour
{
    private static event EventHandler<UpgradeChangedEventArgs> onUpgradeChanged;

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
    private MoonpoolOccupiedHandler occupiedHandler;
    
    private RectTransform rectTransform;
    private UpgradeScreen upgradeScreen;
    private ProtoUpgradeManager upgradeManager;
    private Vector2 originalSize;
    private bool hovered;
    private bool pointerDownLastFrame;
    private bool craftTriggered;
    private bool allowedToCraft = true;
    private bool hadSubLastFrame;
    private float currentConfirmTime;
    private float oldTooltipScale;

    private Atlas.Sprite atlasSpriteBGNormal;
    private Atlas.Sprite atlasSpriteBGHovered;
    private bool initialized;

    private void Start()
    {
        buildScreen = GetComponentInParent<BuildTerminalScreenManager>().GetComponentInChildren<uGUI_ProtoBuildScreen>(true);
        upgradeScreen = GetComponentInParent<UpgradeScreen>();

        atlasSpriteBGNormal = new Atlas.Sprite(backgroundNormalSprite);
        atlasSpriteBGHovered = new Atlas.Sprite(backgroundHoverSprite);

        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;

        occupiedHandler = buildScreen.GetMoonpoolHandler();

        OnSubInMoonpoolChanged();
        SetUpgradeTechType(techType.TechType);

        UWE.CoroutineHost.StartCoroutine(RefreshUpgrades());
        initialized = true;
    }

    private IEnumerator RefreshUpgrades()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (upgradeManager == null) yield break;

        OnUpgradesChanged(null, new UpgradeChangedEventArgs(upgradeScreen, upgradeManager.GetInstalledUpgradeTypes()));
    }

    private void OnEnable()
    {
        onUpgradeChanged += OnUpgradesChanged;

        if (!initialized)
        {
            CoroutineHost.StartCoroutine(LateInitialize());
        }
    }

    private IEnumerator LateInitialize()
    {
        yield return new WaitForEndOfFrame();

        if (!upgradeManager) yield break;

        SetUpgradeTechType(techType.TechType);
        if (upgradeManager.GetInstalledUpgradeTypes().Contains(techType.TechType))
        {
            upgradeScreen.InstallUpgrade(this);
        }

        UWE.CoroutineHost.StartCoroutine(RefreshUpgrades());
        initialized = true;
    }

    private void OnDisable()
    {
        onUpgradeChanged -= OnUpgradesChanged;
    }

    private void OnSubInMoonpoolChanged()
    {
        if (occupiedHandler.SubInMoonpool == null)
        {
            upgradeManager = null;
            return;
        }

        upgradeManager = occupiedHandler.SubInMoonpool.GetComponentInChildren<ProtoUpgradeManager>();
        
        if (upgradeManager.GetInstalledUpgradeTypes().Contains(techType.TechType))
        {
            upgradeScreen.InstallUpgrade(this);
        }

        OnUpgradesChanged(null, new UpgradeChangedEventArgs(upgradeScreen, upgradeManager.GetInstalledUpgradeTypes()));
    }

    public void SetUpgradeTechType(TechType techType)
    {
        tooltip.rocketTechType = techType;

        itemIcon.SetForegroundSprite(SpriteManager.Get(techType));
        InitialzeFGIcon(itemIcon);

        itemIcon.SetBackgroundSprite(atlasSpriteBGNormal);
        InitializeBGIcon(itemIcon);
    }

    private void Update()
    {
        if (occupiedHandler.MoonpoolHasSub != hadSubLastFrame)
        {
            OnSubInMoonpoolChanged();
        }

        if (!allowedToCraft)
        {
            hadSubLastFrame = occupiedHandler.MoonpoolHasSub;
            return;
        }

        bool pointerDown = GameInput.GetButtonHeld(GameInput.Button.LeftHand);

        if (hovered && pointerDown)
        {
            HandleConfirmCountdown();
        }
        else
        {
            currentConfirmTime = -1;
        }

        HandleHoverScale();

        progressMask.fillAmount = currentConfirmTime / (currentConfirmTime == -1 ? currentConfirmTime : confirmTime);
        pointerDownLastFrame = pointerDown;
        
        hadSubLastFrame = occupiedHandler.MoonpoolHasSub;
    }

    private void FixedUpdate()
    {
        tooltip.gameObject.SetActive(allowedToCraft);
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
            OnActionConfirmed();
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
        if (!allowedToCraft) return;

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
        if (icon.background == null) return;

        var rt = icon.background.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.sizeDelta = new Vector2(0.003f, 0.003f);
    }

    private void InitialzeFGIcon(uGUI_ItemIcon icon)
    {
        if (icon.foreground == null) return;

        var rt = icon.foreground.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.sizeDelta = new Vector2(0.001f, 0.001f);
        rt.localScale = Vector3.one * 0.6f;
    }

    private void OnActionConfirmed()
    {
        if (!CrafterLogic.ConsumeResources(techType.TechType))
        {
            currentConfirmTime = 0;
            craftTriggered = false;
            return;
        }

        currentConfirmTime = confirmTime;
        craftTriggered = true;

        bool currentlyInstalled = upgradeManager.GetUpgradeInstalled(techType.TechType);
        upgradeManager.SetUpgradeInstalled(techType.TechType, !currentlyInstalled);

        if (!currentlyInstalled)
        {
            upgradeScreen.InstallUpgrade(this);
        }
        else
        {
            upgradeScreen.UninstallUpgrade(this);
        }

        UpgradeChangedEventArgs args = new(upgradeScreen, upgradeManager.GetInstalledUpgradeTypes());
        onUpgradeChanged?.Invoke(this, args);
    }

    private void OnUpgradesChanged(object sender, UpgradeChangedEventArgs args)
    {
        if (args.owner != upgradeScreen) return;

        bool canUseButton = !args.installedUpgrades.Contains(techType.TechType);

        // Disable installation button
        tooltip.gameObject.SetActive(canUseButton);
        float alpha = canUseButton ? 1 : 0.3f;
        itemIcon.SetForegroundAlpha(alpha);
        itemIcon.SetBackgroundAlpha(alpha);

        allowedToCraft = canUseButton;
    }
    
    public TechType GetUpgradeTechType() => techType.TechType;
}

internal class UpgradeChangedEventArgs : EventArgs
{
    public UpgradeScreen owner;
    public List<TechType> installedUpgrades;

    public UpgradeChangedEventArgs(UpgradeScreen owner, List<TechType> installedUpgrades)
    {
        this.owner = owner;
        this.installedUpgrades = installedUpgrades;
    }
}