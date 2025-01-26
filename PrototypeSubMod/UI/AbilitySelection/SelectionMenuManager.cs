using PrototypeSubMod.Patches;
using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using SubLibrary.UI;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.UI.AbilitySelection;

internal class SelectionMenuManager : MonoBehaviour, IUIElement
{
    [SerializeField] private List<GameObject> abilities;
    [SerializeField] private IconDistributor distributor;
    [SerializeField] private TetherManager tetherManager;
    [SerializeField] private ProtoUpgradeManager upgradeManager;
    [SerializeField] private Transform cameraFocus;
    [SerializeField] private float maxAlignmentSpeed;

    [SerializeField, HideInInspector] public List<IAbilityIcon> abilityIcons = new();
    private List<IAbilityIcon> iconsToShow = new();
    private bool menuEnabled;

    private void OnValidate()
    {
        abilityIcons.Clear();

        for (int i = abilities.Count - 1; i >= 0; i--)
        {
            var ability = abilities[i].GetComponent<IAbilityIcon>();
            if (ability == null)
            {
                abilities.RemoveAt(i);
            }
            else
            {
                abilityIcons.Add(ability);
            }
        }
    }

    private void Start()
    {
        AssignIcons();
        RefreshIcons();
        upgradeManager.onInstalledUpgradesChanged += RefreshIcons;
    }

    private void RetrieveIconsToShow()
    {
        iconsToShow.Clear();
        foreach (var ability in abilityIcons)
        {
            if (!ability.GetShouldShow()) continue;
            iconsToShow.Add(ability);
        }
    }

    private void RefreshIcons()
    {
        RetrieveIconsToShow();
        distributor.RegenerateIcons(iconsToShow);
    }

    private void AssignIcons()
    {
        foreach (var item in abilities)
        {
            var ability = item.GetComponent<IAbilityIcon>();
            abilityIcons.Add(ability);
        }
    }

    public void UpdateUI()
    {
        if (!menuEnabled)
        {
            MainCameraControl_Patches.SetOverwriteDelta(Vector2.zero, false);
            return;
        }

        MainCameraControl_Patches.SetOverwriteDelta(ProtoCameraUtils.CalculateTargetAngleDelta(cameraFocus, maxAlignmentSpeed), true);
    }

    public void SetMenuEnabled(bool enabled)
    {
        tetherManager.SetMenuOpen(enabled);
        menuEnabled = enabled;
    }

    private void OnDestroy()
    {
        MainCameraControl_Patches.SetOverwriteDelta(Vector2.zero, false);
    }

    public void OnSubDestroyed()
    {
        MainCameraControl_Patches.SetOverwriteDelta(Vector2.zero, false);
    }
}
