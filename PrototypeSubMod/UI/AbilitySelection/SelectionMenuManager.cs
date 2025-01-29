﻿using PrototypeSubMod.Patches;
using PrototypeSubMod.Upgrades;
using PrototypeSubMod.Utility;
using SubLibrary.UI;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.UI.AbilitySelection;

internal class SelectionMenuManager : MonoBehaviour, IUIElement
{
    [SerializeField] private List<GameObject> abilities;
    [SerializeField] private int defaultAbilityIndex;
    [SerializeField] private IconDistributor distributor;
    [SerializeField] private TetherManager tetherManager;
    [SerializeField] private ProtoUpgradeManager upgradeManager;
    [SerializeField] private Animator menuManager;
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
        tetherManager.onAbilitySelected += () => SetMenuEnabled(false);

        tetherManager.SelectIcon(distributor.GetIconAtIndex(defaultAbilityIndex).GetComponent<RadialIcon>(), true);
    }

    private void Update()
    {
        if (!GameInput.GetButtonDown(GameInput.Button.Exit)) return;

        SetMenuEnabled(false);
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
        var selectedAbility = tetherManager.GetSelectedIcon().GetAbility();
        if (!selectedAbility.GetShouldShow())
        {
            selectedAbility.OnSelectedChanged(false);
            tetherManager.SelectIcon(distributor.GetIconAtIndex(defaultAbilityIndex).GetComponent<RadialIcon>(), true);
        }

        RetrieveIconsToShow();
        distributor.RegenerateIcons(iconsToShow);
        tetherManager.RegenerateHighlightArc();
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
        if (!menuEnabled) return;

        MainCameraControl_Patches.SetOverwriteDelta(ProtoCameraUtils.CalculateTargetAngleDelta(cameraFocus, maxAlignmentSpeed), true);
    }

    public void SetMenuEnabled(bool enabled)
    {
        tetherManager.SetMenuOpen(enabled);
        menuEnabled = enabled;
        menuManager.SetBool("MenuOpen", enabled);

        if (!enabled)
        {
            MainCameraControl_Patches.SetOverwriteDelta(Vector2.zero, false);
        }
    }

    public void OpenMenu()
    {
        if (menuEnabled) return;

        SetMenuEnabled(true);
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
