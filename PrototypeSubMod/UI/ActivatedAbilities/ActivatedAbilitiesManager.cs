using PrototypeSubMod.UI.AbilitySelection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PrototypeSubMod.UI.ActivatedAbilities;

internal class ActivatedAbilitiesManager : MonoBehaviour
{
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform iconsParent;
    [SerializeField] private SelectionMenuManager menuManager;
    [SerializeField] private TetherManager tetherManager;

    private List<ActiveAbilityIcon> activeAbilityIcons = new();

    private void Start()
    {
        tetherManager.onAbilityActivatedChanged += OnAbilitySelectedChanged;
    }

    private void OnAbilitySelectedChanged(IAbilityIcon icon)
    {
        var activeIcon = activeAbilityIcons.FirstOrDefault(i => i != null && i.GetIcon() == icon);
        bool isActive = activeIcon != null && activeIcon.GetIcon().GetActive();

        if (activeIcon == null)
        {
            var newIcon = CreateNewIcon(icon);
            activeAbilityIcons.Add(newIcon);
            activeIcon = newIcon;
            isActive = icon.GetActive();
        }

        if (!activeIcon.gameObject.activeSelf && isActive)
        {
            activeIcon.gameObject.SetActive(true);
        }
        else if (activeIcon && !isActive)
        {
            activeIcon.gameObject.SetActive(false);
        }
    }

    private ActiveAbilityIcon CreateNewIcon(IAbilityIcon icon)
    {
        var instance = Instantiate(iconPrefab, iconsParent);
        var newIcon = instance.GetComponent<ActiveAbilityIcon>();
        newIcon.SetIcon(icon);

        return newIcon;
    }
}
