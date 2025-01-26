using PrototypeSubMod.Upgrades;
using System.Collections.Generic;
using UnityEngine;

namespace PrototypeSubMod.UI.AbilitySelection;

internal class SelectionMenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> abilities;
    [SerializeField] private IconDistributor distributor;
    [SerializeField] private ProtoUpgradeManager upgradeManager;

    [SerializeField, HideInInspector] public List<IAbilityIcon> abilityIcons = new();
    private List<IAbilityIcon> iconsToShow = new();

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
}
