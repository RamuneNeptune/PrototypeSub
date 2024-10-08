using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PrototypeSubMod.SubTerminal;

internal class uGUI_ProtoUpgradeIcon : MonoBehaviour
{
    private static event EventHandler<UpgradeChangedEventArgs> onUpgradeChanged;
    private static Dictionary<string, TechType> uninstallationTechTypes = new();

    private string UpgradeIdentifier
    {
        get
        {
            return transform.parent.parent.name + "_" + gameObject.name;
        }
    }

    private TechType CurrentTechType
    {
        get
        {
            if (ProtoUpgradeManager.Instance == null) return techType.TechType;

            return ProtoUpgradeManager.Instance.GetUpgradeInstalled(techType.TechType) ? uninstallationTechType : techType.TechType;
        }
    }

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
    public Sprite uninstallSprite;

    private uGUI_ProtoBuildScreen buildScreen;
    private CrafterLogic crafterLogic;
    private TechType uninstallationTechType;

    private bool hovered;
    private bool pointerDownLastFrame;
    private bool craftTriggered;
    private bool allowedToCraft = true;
    private float currentConfirmTime;
    private float oldTooltipScale;
    private Vector2 originalSize;
    private RectTransform rectTransform;
    private UpgradeScreen upgradeScreen;

    private Atlas.Sprite atlasSpriteBGNormal;
    private Atlas.Sprite atlasSpriteBGHovered;

    private void Start()
    {
        buildScreen = GetComponentInParent<uGUI_ProtoBuildScreen>();
        upgradeScreen = GetComponentInParent<UpgradeScreen>();
        uninstallationTechType = uninstallationTechTypes[UpgradeIdentifier];

        atlasSpriteBGNormal = new Atlas.Sprite(backgroundNormalSprite);
        atlasSpriteBGHovered = new Atlas.Sprite(backgroundHoverSprite);

        rectTransform = GetComponent<RectTransform>();
        crafterLogic = GetComponent<CrafterLogic>();
        originalSize = rectTransform.sizeDelta;

        SetUpgradeTechType(CurrentTechType);
        if (ProtoUpgradeManager.Instance.GetInstalledUpgrades().Contains(techType.TechType))
        {
            upgradeScreen.InstallUpgrade(this);
        }

        UWE.CoroutineHost.StartCoroutine(RefreshUpgrades());
    }

    private IEnumerator RefreshUpgrades()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        OnUpgradesChanged(null, new UpgradeChangedEventArgs(upgradeScreen, ProtoUpgradeManager.Instance.GetInstalledUpgrades()));
    }

    private void OnEnable()
    {
        onUpgradeChanged += OnUpgradesChanged;
    }

    private void OnDisable()
    {
        onUpgradeChanged -= OnUpgradesChanged;
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
        if (!buildScreen.IsTooltipActive() || !allowedToCraft) return;

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
    }

    private void FixedUpdate()
    {
        tooltip.gameObject.SetActive(buildScreen.IsTooltipActive() && allowedToCraft);
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
        if (!buildScreen.IsTooltipActive() || !allowedToCraft) return;

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

    private void InitialzeFGIcon(uGUI_ItemIcon icon)
    {
        var rt = icon.foreground.GetComponent<RectTransform>();
        rt.anchorMax = Vector2.one;
        rt.anchorMin = Vector2.zero;
        rt.sizeDelta = new Vector2(0.001f, 0.001f);
        rt.localScale = Vector3.one * 0.6f;
    }

    private void OnActionConfirmed()
    {
        if (!CrafterLogic.ConsumeResources(CurrentTechType))
        {
            currentConfirmTime = 0;
            craftTriggered = false;
            return;
        }

        currentConfirmTime = confirmTime;
        craftTriggered = true;

        bool currentlyInstalled = ProtoUpgradeManager.Instance.GetUpgradeInstalled(techType.TechType);
        ProtoUpgradeManager.Instance.SetUpgradeInstalled(techType.TechType, !currentlyInstalled);

        ErrorMessage.AddError($"Changing {techType.TechType} to installed = {!currentlyInstalled}");

        if (!currentlyInstalled)
        {
            upgradeScreen.InstallUpgrade(this);
        }
        else
        {
            upgradeScreen.UninstallUpgrade(this);
        }

        UpgradeChangedEventArgs args = new(upgradeScreen, ProtoUpgradeManager.Instance.GetInstalledUpgrades());
        onUpgradeChanged?.Invoke(this, args);

        if (currentlyInstalled)
        {
            // Has just been uninstalled
            crafterLogic.timeCraftingBegin = 0;
            crafterLogic.timeCraftingEnd = 1;
            crafterLogic.craftingTechType = uninstallationTechType;
            UWE.CoroutineHost.StartCoroutine(crafterLogic.TryPickupAsync());
        }

        InitialzeFGIcon(itemIcon);
    }

    private void OnUpgradesChanged(object sender, UpgradeChangedEventArgs args)
    {
        if (args.owner != upgradeScreen) return;

        bool canUseButton = upgradeScreen.CanInstallNewUpgrade() || args.installedUpgrades.Contains(techType.TechType);

        // Disable installation button
        tooltip.gameObject.SetActive(canUseButton);
        float alpha = canUseButton ? 1 : 0.3f;
        itemIcon.SetForegroundAlpha(alpha);
        itemIcon.SetBackgroundAlpha(alpha);

        allowedToCraft = canUseButton;
        SetUpgradeTechType(CurrentTechType);
    }

    public void SetUninstallationTechType(TechType techType)
    {
        uninstallationTechTypes.Add(UpgradeIdentifier, techType);
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