using System;
using UnityEngine;

namespace PrototypeSubMod.SubTerminal;

[RequireComponent(typeof(uGUI_ItemIcon)), RequireComponent(typeof(RocketBuilderTooltip))]
internal class uGUI_ProtoUpgradeIcon : MonoBehaviour
{
    public Action<TechType> onCraftPressed;

    private uGUI_ItemIcon itemIcon;
    private RocketBuilderTooltip tooltip;
    private TechType techType;

    private void Start()
    {
        itemIcon = GetComponent<uGUI_ItemIcon>();
        tooltip = GetComponent<RocketBuilderTooltip>();
    }

    public void SetUpgradeTechType(TechType techType)
    {
        this.techType = techType;
        tooltip.rocketTechType = techType;
        itemIcon.SetForegroundSprite(SpriteManager.Get(techType));

        var backgroundSprite = SpriteManager.GetBackground(CraftData.GetBackgroundType(techType));
        itemIcon.SetBackgroundSprite(backgroundSprite);
    }

    public void OnCraftPressed()
    {
        onCraftPressed?.Invoke(techType);
    }
}
